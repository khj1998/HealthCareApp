using System;
using UnityEngine;
using Mediapipe.Unity;
using Mediapipe.Unity.CoordinateSystem;

namespace SOMA.MediaPipe
{
	public struct PoseConfig
	{
		// If set to false, the solution treats the input images as a video stream.
		// It will try to detect the most prominent person in the very first images, and upon a successful detection further localizes the pose landmarks.
		// In subsequent images, it then simply tracks those landmarks without invoking another detection until it loses track, on reducing computation and latency.
		// If set to true, person detection runs every input image, ideal for processing a batch of static, possibly unrelated, images.
		// Default to false.
		public bool StaticImageMode;
		// If set to true, the solution filters pose landmarks across different input images to reduce jitter.
		// Ignored if static_image_mode is also set to true.
		public bool SmoothLandmarks;
		// If set to true, in addition to the pose landmarks the solution also generates the segmentation mask.
		public bool EnableSegmentation;
		// If set to true, the solution filters segmentation masks across different input images to reduce jitter.
		// Ignored if enable_segmentation is false or static_image_mode is true.
		public bool SmoothSegmentation;

		// Minimum confidence value ([0.0, 1.0]) from the person-detection model for the detection to be considered successful.
		private float _minDetectionConfidence;
		public float MinDetectionConfidence
		{
			get => _minDetectionConfidence;
			set => _minDetectionConfidence = Mathf.Clamp01(value);
		}
		// Minimum confidence value ([0.0, 1.0]) from the landmark-tracking model for the pose landmarks to be considered tracked successfully,
		// or otherwise person detection will be invoked automatically on the next input image.
		// Setting it to a higher value can increase robustness of the solution, at the expense of a higher latency.
		// Ignored if static_image_mode is true.
		private float _minTrackingConfidence;
		public float MinTrackingConfidence
		{
			get => _minTrackingConfidence;
			set => _minTrackingConfidence = Mathf.Clamp01(value);
		}
		private long _timeoutMicroseconds;
		public long TimeoutMicroseconds
		{
			get => _timeoutMicroseconds;
			set => Math.Max(0, value);
		}

		public PoseConfig(
				bool staticImageMode,
				bool smoothLandmarks,
				bool enableSegmentation,
				bool smoothSegmentation,
				float minDetectionConfidence,
				float minTrackingConfidence,
				long timeoutMicroseconds
			) : this()
		{
			this.StaticImageMode = staticImageMode;
			this.SmoothLandmarks = smoothLandmarks;
			this.EnableSegmentation = enableSegmentation;
			this.SmoothSegmentation = smoothSegmentation;

			this.MinDetectionConfidence = minDetectionConfidence;
			this.MinTrackingConfidence = minTrackingConfidence;
			this.TimeoutMicroseconds = timeoutMicroseconds;
		}

		// 멍청한 유니티가 C#10 지원 안해서 이렇게 해야함
		public static PoseConfig DefaultConfig => new PoseConfig(
			false,
			true,
			false,
			true,
			0.5f,
			0.5f,
			50000L // 50ms
		);
	}
}
