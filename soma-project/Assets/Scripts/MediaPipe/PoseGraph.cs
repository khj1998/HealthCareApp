using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Mediapipe;
using Mediapipe.Unity;
using Google.Protobuf;
using SOMA.Managers;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace SOMA.MediaPipe
{
	public class PoseGraph : MonoBehaviourSingleton<PoseGraph>
	{
		private enum ConfigType
		{
			CPU,
			GPU,
			OpenGLES
		}

		private const string CPU_CONFIG_PATH = "Resources/MediaPipe/pose_tracking_cpu.txt";
		private const string GPU_CONFIG_PATH = "Resources/MediaPipe/pose_tracking_gpu.txt";
		private const string OPENGLES_CONFIG_PATH = "Resources/MediaPipe/pose_tracking_opengles.txt";

		private const string INPUT_STREAM_NAME = "input_video";
		private const string POSE_DETECTION_STREAM_NAME = "pose_detection";
		private const string POSE_LANDMARKS_STREAM_NAME = "pose_landmarks";
		private const string POSE_WORLD_LANDMARKS_STREAM_NAME = "pose_world_landmarks";
		private const string SEGMENTATION_MASK_STREAM_NAME = "segmentation_mask";
		private const string ROI_FROM_LANDMARKS_STREAM_NAME = "roi_from_landmarks";

		private OutputStream<DetectionPacket, Detection> _poseDetectionStream;
		private OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList> _poseLandmarksStream;
		private OutputStream<LandmarkListPacket, LandmarkList> _poseWorldLandmarksStream;
		private OutputStream<ImageFramePacket, ImageFrame> _segmentationMaskStream;
		private OutputStream<NormalizedRectPacket, NormalizedRect> _roiFromLandmarksStream;

		public event EventHandler<OutputEventArgs<Detection>> OnPoseDetectionOutput
		{
			add => _poseDetectionStream.AddListener(value);
			remove => _poseDetectionStream.RemoveListener(value);
		}
		public event EventHandler<OutputEventArgs<NormalizedLandmarkList>> OnPoseLandmarksOutput
		{
			add => _poseLandmarksStream.AddListener(value);
			remove => _poseLandmarksStream.RemoveListener(value);
		}
		public event EventHandler<OutputEventArgs<LandmarkList>> OnPoseWorldLandmarksOutput
		{
			add => _poseWorldLandmarksStream.AddListener(value);
			remove => _poseWorldLandmarksStream.RemoveListener(value);
		}
		public event EventHandler<OutputEventArgs<ImageFrame>> OnSegmentationMaskOutput
		{
			add => _segmentationMaskStream.AddListener(value);
			remove => _segmentationMaskStream.RemoveListener(value);
		}
		public event EventHandler<OutputEventArgs<NormalizedRect>> OnRoiFromLandmarksOutput
		{
			add => _roiFromLandmarksStream.AddListener(value);
			remove => _roiFromLandmarksStream.RemoveListener(value);
		}

		private ConfigType _configType
		{
			get {
				if (GpuManager.IsInitialized)
				{
#if UNITY_ANDROID
					if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
					{
						return ConfigType.OpenGLES;
					}
#endif
					return ConfigType.GPU;
				}

				return ConfigType.CPU;
			}
		}

		private string _textConfigPath
		{
			get {
				switch (_configType)
				{
					case ConfigType.CPU: return CPU_CONFIG_PATH;
					case ConfigType.GPU: return GPU_CONFIG_PATH;
					case ConfigType.OpenGLES: return OPENGLES_CONFIG_PATH;
					default: return null;
				}
			}
		}

		private long _currentMicroseconds => _stopwatch.ElapsedTicks / (System.TimeSpan.TicksPerMillisecond / 1000);

		private Stopwatch _stopwatch;
		private CalculatorGraph _graph;
		private long _timeoutMicroseconds;

		[SerializeField]
		private PoseConfigComponent _poseConfigComponent;
		private PoseConfig _poseConfig;
		private string _textConfig;
		public bool IsReady { get; private set; } = false;

		protected override void WhenAwake() {}

		private IEnumerator Start()
		{
			_poseConfig = _poseConfigComponent.ToPoseConfig();

			yield return Initialize();

			IsReady = true;
		}

		private IEnumerator Initialize()
		{
			yield return PrepareModelFiles();

			// GpuManager.Initialize를 해봐야 config type 결정가능
			yield return GpuManager.Initialize();
			Debug.Log($"Graph config type: {_configType}");

			yield return AdressablesUtility.LoadAssetAsync<TextAsset>(_textConfigPath,
				textConfigAsset => _textConfig = textConfigAsset.text);
		}

		private IEnumerator PrepareModelFiles()
		{
			Debug.Log($"Fetching pose landmark model...");

			var resourceManager = new StreamingAssetsResourceManager("MediaPipe");
			yield return resourceManager.PrepareAssetAsync("pose_detection.bytes");
			yield return resourceManager.PrepareAssetAsync("pose_landmark_lite.bytes");
		}

		private void InitializeGraph(string textConfig, SidePacket sidePacket)
		{
			float minDetectionConfidence = _poseConfig.MinDetectionConfidence;
			float minTrackingConfidence = _poseConfig.MinTrackingConfidence;

			_graph = new CalculatorGraph();

			var baseConfig = CalculatorGraphConfig.Parser.ParseFromTextFormat(textConfig);
			InitializeStreams(baseConfig);

			// using declaration, auto dispose
			using var validatedGraphConfig = new ValidatedGraphConfig();
			validatedGraphConfig.Initialize(baseConfig).AssertOk();

			var extensionRegistry = new ExtensionRegistry() { TensorsToDetectionsCalculatorOptions.Extensions.Ext, ThresholdingCalculatorOptions.Extensions.Ext };
			var cannonicalizedConfig = validatedGraphConfig.Config(extensionRegistry);
			var tensorsToDetectionsCalculators = cannonicalizedConfig.Node.Where(node => node.Calculator == "TensorsToDetectionsCalculator").ToList();
			var thresholdingCalculators = cannonicalizedConfig.Node.Where(node => node.Calculator == "ThresholdingCalculator").ToList();

			foreach (var calculator in tensorsToDetectionsCalculators)
			{
				if (calculator.Options.HasExtension(TensorsToDetectionsCalculatorOptions.Extensions.Ext))
				{
					var options = calculator.Options.GetExtension(TensorsToDetectionsCalculatorOptions.Extensions.Ext);
					options.MinScoreThresh = minDetectionConfidence;
					Debug.Log($"Setting Min Detection Confidence = {minDetectionConfidence}");
				}
			}

			foreach (var calculator in thresholdingCalculators)
			{
				if (calculator.Options.HasExtension(ThresholdingCalculatorOptions.Extensions.Ext))
				{
					var options = calculator.Options.GetExtension(ThresholdingCalculatorOptions.Extensions.Ext);
					options.Threshold = minTrackingConfidence;
					Debug.Log($"Setting Min Tracking Confidence = {minTrackingConfidence}");
				}
			}

			_graph.Initialize(cannonicalizedConfig, sidePacket).AssertOk();

			if (_configType != ConfigType.CPU)
			{
				Debug.Log($"Initializing GPU resources...");
				_graph.SetGpuResources(GpuManager.GpuResources).AssertOk();
			}
		}

		private void InitializeStreams(CalculatorGraphConfig config)
		{
			Debug.Log($"Setting Timeout Microseconds = {_timeoutMicroseconds}");

			_poseDetectionStream = new OutputStream<DetectionPacket, Detection>(_graph, POSE_DETECTION_STREAM_NAME, config.AddPacketPresenceCalculator(POSE_DETECTION_STREAM_NAME), _timeoutMicroseconds);
			_poseLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(_graph, POSE_LANDMARKS_STREAM_NAME, config.AddPacketPresenceCalculator(POSE_LANDMARKS_STREAM_NAME), _timeoutMicroseconds);
			_poseWorldLandmarksStream = new OutputStream<LandmarkListPacket, LandmarkList>(_graph, POSE_WORLD_LANDMARKS_STREAM_NAME, config.AddPacketPresenceCalculator(POSE_WORLD_LANDMARKS_STREAM_NAME), _timeoutMicroseconds);
			_segmentationMaskStream = new OutputStream<ImageFramePacket, ImageFrame>(_graph, SEGMENTATION_MASK_STREAM_NAME, config.AddPacketPresenceCalculator(SEGMENTATION_MASK_STREAM_NAME), _timeoutMicroseconds);
			_roiFromLandmarksStream = new OutputStream<NormalizedRectPacket, NormalizedRect>(_graph, ROI_FROM_LANDMARKS_STREAM_NAME, config.AddPacketPresenceCalculator(ROI_FROM_LANDMARKS_STREAM_NAME), _timeoutMicroseconds);
		}

		private SidePacket BuildSidePacket(PoseConfig poseConfig, PoseImageConfig poseImageConfig)
		{
			var sidePacket = new SidePacket();

			Debug.Log($"Setting Input Rotation = {poseImageConfig.InputRotation}");
			sidePacket.Emplace("input_rotation", new IntPacket(poseImageConfig.InputRotation));
			Debug.Log($"Setting Input Horizontally Flipped = {poseImageConfig.InputHorizontallyFlipped}");
			sidePacket.Emplace("input_horizontally_flipped", new BoolPacket(poseImageConfig.InputHorizontallyFlipped));
			Debug.Log($"Setting Input Vertically Flipped = {poseImageConfig.InputVerticallyFlipped}");
			sidePacket.Emplace("input_vertically_flipped", new BoolPacket(poseImageConfig.InputVerticallyFlipped));

			Debug.Log($"Setting Output Rotation = {poseImageConfig.OutputRotation}");
			sidePacket.Emplace("output_rotation", new IntPacket(poseImageConfig.OutputRotation));
			Debug.Log($"Setting Output Horizontally Flipped = {poseImageConfig.OutputHorizontallyFlipped}");
			sidePacket.Emplace("output_horizontally_flipped", new BoolPacket(poseImageConfig.OutputHorizontallyFlipped));
			Debug.Log($"Setting Output Vertically Flipped = {poseImageConfig.OutputVerticallyFlipped}");
			sidePacket.Emplace("output_vertically_flipped", new BoolPacket(poseImageConfig.OutputVerticallyFlipped));

			Debug.Log($"Setting Static Image Mode = {poseConfig.StaticImageMode}");
			sidePacket.Emplace("use_prev_landmarks", new BoolPacket(!poseConfig.StaticImageMode));
			Debug.Log($"Setting Model Complexity = Lite");
			sidePacket.Emplace("model_complexity", new IntPacket(0));
			Debug.Log($"Setting Smooth Landmarks = {poseConfig.SmoothLandmarks}");
			sidePacket.Emplace("smooth_landmarks", new BoolPacket(poseConfig.SmoothLandmarks));
			Debug.Log($"Setting Enable Segmentation = {poseConfig.EnableSegmentation}");
			sidePacket.Emplace("enable_segmentation", new BoolPacket(poseConfig.EnableSegmentation));
			Debug.Log($"Setting Smooth Segmentation = {poseConfig.SmoothSegmentation}");
			sidePacket.Emplace("smooth_segmentation", new BoolPacket(poseConfig.SmoothSegmentation));

			return sidePacket;
		}

		public void Run(PoseImageConfig poseImageConfig)
		{
			_timeoutMicroseconds = _poseConfig.TimeoutMicroseconds;
			_stopwatch = new Stopwatch();

			Debug.Log("Initializing Graph...");

			SidePacket sidePacket = BuildSidePacket(_poseConfig, poseImageConfig);
			InitializeGraph(_textConfig, sidePacket);

			Debug.Log("Starting Graph...");

			_poseDetectionStream.StartPolling().AssertOk();
			_poseLandmarksStream.StartPolling().AssertOk();
			_poseWorldLandmarksStream.StartPolling().AssertOk();
			_segmentationMaskStream.StartPolling().AssertOk();
			_roiFromLandmarksStream.StartPolling().AssertOk();

			_stopwatch.Start();
			_graph.StartRun().AssertOk();
		}

		public void AddTextureToInputStream(Texture2D texture)
		{
			var timestamp = new Timestamp(_currentMicroseconds);

			if (_configType == ConfigType.OpenGLES)
			{
				#if UNITY_ANDROID
				var gpuBuffer = texture.ToGpuBuffer(GpuManager.GlCalculatorHelper.GetGlContext());
				var packet = new GpuBufferPacket(gpuBuffer, timestamp);
				_graph.AddPacketToInputStream(INPUT_STREAM_NAME, packet).AssertOk();
				#endif
			}
			else
			{
				var imageFrame = texture.ToImageFrame();
				var packet = new ImageFramePacket(imageFrame, timestamp);
				_graph.AddPacketToInputStream(INPUT_STREAM_NAME, packet).AssertOk();
			}
		}

		public bool TryGetOutput(out Detection poseDetection, out NormalizedLandmarkList poseLandmarks, out LandmarkList poseWorldLandmarks, out ImageFrame segmentationMask, out NormalizedRect roiFromLandmarks, bool allowBlock)
		{
			var currentMicroseconds = _currentMicroseconds;
			var r1 = TryGetPoseDetection(out poseDetection, allowBlock, currentMicroseconds);
			var r2 = TryGetPoseLandmarks(out poseLandmarks, allowBlock, currentMicroseconds);
			var r3 = TryGetPoseWorldLandmarks(out poseWorldLandmarks, allowBlock, currentMicroseconds);
			var r4 = TryGetSegmentationMask(out segmentationMask, allowBlock, currentMicroseconds);
			var r5 = TryGetRoiFromLandmarks(out roiFromLandmarks, allowBlock, currentMicroseconds);

			return r1 || r2 || r3 || r4 || r5;
		}

		public bool TryGetPoseDetection(out Detection poseDetection, bool allowBlock) => TryGetPoseDetection(out poseDetection, allowBlock, _currentMicroseconds);
		private bool TryGetPoseDetection(out Detection poseDetection, bool allowBlock, long currentMicroseconds) => TryGetNext(_poseDetectionStream, out poseDetection, allowBlock, currentMicroseconds);
		public bool TryGetPoseLandmarks(out NormalizedLandmarkList poseLandmarks, bool allowBlock) => TryGetPoseLandmarks(out poseLandmarks, allowBlock, _currentMicroseconds);
		private bool TryGetPoseLandmarks(out NormalizedLandmarkList poseLandmarks, bool allowBlock, long currentMicroseconds) => TryGetNext(_poseLandmarksStream, out poseLandmarks, allowBlock, currentMicroseconds);
		public bool TryGetPoseWorldLandmarks(out LandmarkList poseWorldLandmarks, bool allowBlock) => TryGetPoseWorldLandmarks(out poseWorldLandmarks, allowBlock, _currentMicroseconds);
		private bool TryGetPoseWorldLandmarks(out LandmarkList poseWorldLandmarks, bool allowBlock, long currentMicroseconds) => TryGetNext(_poseWorldLandmarksStream, out poseWorldLandmarks, allowBlock, currentMicroseconds);
		public bool TryGetSegmentationMask(out ImageFrame segmentationMask, bool allowBlock) => TryGetSegmentationMask(out segmentationMask, allowBlock, _currentMicroseconds);
		private bool TryGetSegmentationMask(out ImageFrame segmentationMask, bool allowBlock, long currentMicroseconds) => TryGetNext(_segmentationMaskStream, out segmentationMask, allowBlock, currentMicroseconds);
		public bool TryGetRoiFromLandmarks(out NormalizedRect roiFromLandmarks, bool allowBlock) => TryGetRoiFromLandmarks(out roiFromLandmarks, allowBlock, _currentMicroseconds);
		private bool TryGetRoiFromLandmarks(out NormalizedRect roiFromLandmarks, bool allowBlock, long currentMicroseconds) => TryGetNext(_roiFromLandmarksStream, out roiFromLandmarks, allowBlock, currentMicroseconds);

		private bool TryGetNext<TPacket, TValue>(OutputStream<TPacket, TValue> stream, out TValue value, bool allowBlock, long currentMicroseconds) where TPacket : Packet<TValue>, new()
		{
			var result = stream.TryGetNext(out value, allowBlock);
			return result || allowBlock || stream.ResetTimestampIfTimedOut(currentMicroseconds, _timeoutMicroseconds);
		}

		public void Stop()
		{
			Debug.Log("Stopping Graph...");

			if (_stopwatch?.IsRunning == true)
			{
				_stopwatch.Stop();
				_stopwatch = null;
			}

			_poseDetectionStream?.Close();
			_poseDetectionStream = null;
			_poseLandmarksStream?.Close();
			_poseLandmarksStream = null;
			_poseWorldLandmarksStream?.Close();
			_poseWorldLandmarksStream = null;
			_segmentationMaskStream?.Close();
			_segmentationMaskStream = null;
			_roiFromLandmarksStream?.Close();
			_roiFromLandmarksStream = null;

			if (_graph is not null)
			{
				try
				{
					_graph.CloseAllPacketSources().AssertOk();
					_graph.WaitUntilDone().AssertOk();
				}
				finally
				{
					_graph.Dispose();
					_graph = null;
				}
			}
		}

		protected override void WhenDestroy()
		{
			Stop();

			if (GpuManager.IsInitialized)
			{
				GpuManager.Shutdown();
			}
		}
	}
}
