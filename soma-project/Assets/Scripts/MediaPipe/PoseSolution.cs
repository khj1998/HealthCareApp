using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe;
using Mediapipe.Unity;
using SOMA.MediaPipe.Classification;

using BodyPart = SOMA.MediaPipe.Classification.PoseLandmarks.BodyPart;

namespace SOMA.MediaPipe
{
	public class PoseSolution : MonoBehaviour
	{
		public enum PoseRunningMode
		{
			Async,
			NonBlockingSync,
			Sync,
		}

		[SerializeField]
		private RawImage _screen;
		[SerializeField]
		private int _desiredWidth;
		[SerializeField]
		private int _desiredHeight;
		[SerializeField]
		private bool _isInputMirrored;

		[SerializeField]
		private PoseRunningMode _runningMode;
		[SerializeField]
		private PoseLandmarkListAnnotationController _poseLandmarksAnnotationController;
		[SerializeField]
		private PoseClassifierProcessor _poseClassifier;

		public Action<NormalizedLandmarkList> Callback;
		public Action<bool> IsInsideCameraCallback;

		private WebCamTexture _webCamTexture;
		private Texture2D _inputTexture;
		private Color32[] _inputPixelData;

		private int _lostFrames = 0;
		private bool _wasInsideCamera = false;

		public void StartScreen() => _screen.gameObject.SetActive(true);
		public void StopScreen() => _screen.gameObject.SetActive(false);

		public void StartAnnotation() => _poseLandmarksAnnotationController.gameObject.SetActive(true);
		public void StopAnnotation() => _poseLandmarksAnnotationController.gameObject.SetActive(false);

		// Start is called before the first frame update
		private IEnumerator Start()
		{
			try
			{
				var webCamDevice = WebCamTexture.devices.First(x => x.isFrontFacing);
				_webCamTexture = new WebCamTexture(webCamDevice.name, _desiredWidth, _desiredHeight);
				_webCamTexture.Play();
			}
			catch (InvalidOperationException)
			{
				throw new System.Exception("Web Camera devices are not found");
			}

			yield return new WaitUntil(() => _webCamTexture.width > 16);

			int textureWidth = _webCamTexture.width;
			int textureHeight = _webCamTexture.height;

			Debug.Log($"WebCam Found: {textureWidth}x{textureHeight}");

			PoseImageConfig poseImageConfig = new PoseImageConfig(_webCamTexture, _isInputMirrored);

			float screenWidth, screenHeight;

			if (poseImageConfig.InputRotation.IsInverted())
			{
				(screenHeight, screenWidth) = MoreMath.FitScreenSize((textureHeight, textureWidth), (Screen.width, Screen.height));
				(this.transform as RectTransform).sizeDelta = new Vector2(screenHeight, screenWidth);
			}
			else
			{
				(screenWidth, screenHeight) = MoreMath.FitScreenSize((textureWidth, textureHeight), (Screen.width, Screen.height));
				(this.transform as RectTransform).sizeDelta = new Vector2(screenWidth, screenHeight);
			}

			if (_screen is not null)
			{
				Vector3 screenEulerAngles = Vector3.zero;
				if (!poseImageConfig.InputVerticallyFlipped)
				{
					screenEulerAngles.x = 180f;
				}
				if (poseImageConfig.InputHorizontallyFlipped)
				{
					screenEulerAngles.y = 180f;
				}
				screenEulerAngles.z = poseImageConfig.InputRotation.Reverse();

				_screen.rectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);
				_screen.texture = _webCamTexture;
				_screen.rectTransform.eulerAngles = screenEulerAngles;
			}

			_inputTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
			_inputPixelData = new Color32[textureWidth * textureHeight];

			yield return new WaitUntil(() => PoseGraph.Instance.IsReady);

			PoseGraph.Instance.Run(poseImageConfig);

			if (_runningMode == PoseRunningMode.Async)
			{
				PoseGraph.Instance.OnPoseLandmarksOutput += (_, e) => ProcessPoseLandmarks(e.value, false);
			}

			while (true)
			{
				_inputTexture.SetPixels32(_webCamTexture.GetPixels32(_inputPixelData));
				_inputTexture.Apply();
				PoseGraph.Instance.AddTextureToInputStream(_inputTexture);

				if (_runningMode != PoseRunningMode.Async)
				{
					NormalizedLandmarkList poseLandmarks = null;

					if (_runningMode == PoseRunningMode.Sync)
					{
						PoseGraph.Instance.TryGetPoseLandmarks(out poseLandmarks, true);
					}
					else if (_runningMode == PoseRunningMode.NonBlockingSync)
					{
						yield return new WaitUntil(() => PoseGraph.Instance.TryGetPoseLandmarks(out poseLandmarks, false));
					}

					ProcessPoseLandmarks(poseLandmarks, true);
				}

				if (_runningMode != PoseRunningMode.NonBlockingSync)
				{
					yield return null;
				}
			}
		}

		private void ProcessPoseLandmarks(NormalizedLandmarkList poseLandmarks, bool drawNow)
		{
			IsInsideCameraCallback?.Invoke(IsInsideCamera(poseLandmarks));
			if (poseLandmarks is null)
			{
				return;
			}
			if (_poseLandmarksAnnotationController.gameObject.activeInHierarchy)
			{
				if (drawNow)
				{
					_poseLandmarksAnnotationController?.DrawNow(poseLandmarks);
				}
				else
				{
					_poseLandmarksAnnotationController?.DrawLater(poseLandmarks);
				}
			}

			_poseClassifier?.Classify(poseLandmarks);
			Callback?.Invoke(poseLandmarks);
		}

		private bool IsInsideCamera(NormalizedLandmarkList poseLandmarks)
		{
			if (poseLandmarks is null)
			{
				if (++_lostFrames >= 5)
				{
					_wasInsideCamera = false;
				}

				return _wasInsideCamera;
			}

			_lostFrames = 0;

			// X Y는 왼쪽 위가 높음
			NormalizedLandmark lE = poseLandmarks.Landmark[(int)BodyPart.LeftEye];
			NormalizedLandmark rE = poseLandmarks.Landmark[(int)BodyPart.RightEye];
			NormalizedLandmark lw = poseLandmarks.Landmark[(int)BodyPart.LeftWrist];
			NormalizedLandmark rw = poseLandmarks.Landmark[(int)BodyPart.RightWrist];
			NormalizedLandmark la = poseLandmarks.Landmark[(int)BodyPart.LeftAnkle];
			NormalizedLandmark ra = poseLandmarks.Landmark[(int)BodyPart.RightAnkle];

			// 인식 안 된 경우
			if (lE.Y < 0f || rE.Y < 0f || lw.Presence < 0.9f || rw.Presence < 0.9f || la.Presence < 0.9f || ra.Presence < 0.9f)
			{
				return _wasInsideCamera = false;
			}

			NormalizedLandmark le = poseLandmarks.Landmark[(int)BodyPart.LeftElbow];
			NormalizedLandmark re = poseLandmarks.Landmark[(int)BodyPart.RightElbow];
			NormalizedLandmark ls = poseLandmarks.Landmark[(int)BodyPart.LeftShoulder];
			NormalizedLandmark rs = poseLandmarks.Landmark[(int)BodyPart.RightShoulder];
			NormalizedLandmark lh = poseLandmarks.Landmark[(int)BodyPart.LeftHip];
			NormalizedLandmark rh = poseLandmarks.Landmark[(int)BodyPart.RightHip];
			NormalizedLandmark lk = poseLandmarks.Landmark[(int)BodyPart.LeftKnee];
			NormalizedLandmark rk = poseLandmarks.Landmark[(int)BodyPart.RightKnee];

			Point shoulder = Point.Average(new Point(ls), new Point(rs));
			Point hip = Point.Average(new Point(lh), new Point(rh));
			Point knee = Point.Average(new Point(lk), new Point(rk));
			Point ankle = Point.Average(new Point(la), new Point(ra));
			Point diff = shoulder - ankle;

			//Debug.Log($"{Mathf.Abs(Mathf.Atan2(diff.X, diff.Y))} {Point.Angle2D(new Point(ls), new Point(le), new Point(lw))} {Point.Angle2D(new Point(rs), new Point(re), new Point(rw))} {Point.Angle3D(shoulder, hip, ankle)}");
			return _wasInsideCamera =
				diff.Y < -0.29f && Mathf.Abs(Mathf.Atan2(diff.X, diff.Y)) > 2.9f &&
				Point.Angle2D(new Point(ls), new Point(le), new Point(lw)) > 155f &&
				Point.Angle2D(new Point(rs), new Point(re), new Point(rw)) > 155f &&
				Point.Angle3D(shoulder, hip, ankle) > 145f &&
				Point.Angle3D(hip, knee, ankle) > 105f;
		}

		private void OnDestroy()
		{
			PoseGraph.Instance.Stop();
			_webCamTexture?.Stop();
		}
	}
}
