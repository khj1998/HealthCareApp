using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SOMA.firebaseDB;
using SOMA.Managers;
#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

namespace SOMA.StartTasks
{
	public class CameraPermissionTask : StartTask
	{
		[SerializeField]
		private GameObject _board;
		[SerializeField]
		private Button _settingsButton;
		private int _failedCount = 0;

		private void Update()
		{
			if (HasCameraPermission())
			{
				CameraPermissionGranted();
			}
		}

		private bool HasCameraPermission()
		{
			#if UNITY_ANDROID
			return Permission.HasUserAuthorizedPermission(Permission.Camera);
			#elif UNITY_IOS
			return Application.HasUserAuthorization(UserAuthorization.WebCam);
			#endif
		}

		private void CameraPermissionGranted()
		{
			DataManager.Instance.IsPermissionGranted = true;
			_board.SetActive(false);
			FinishTask();
		}

		public void RequestCameraPermission()
		{
			if (!HasCameraPermission())
			{
				#if UNITY_ANDROID
				RequestCameraPermissionAndroid();
				#elif UNITY_IOS
				StartCoroutine(RequestCameraPermissionIOS());
				#endif
			}
		}

		#if UNITY_ANDROID
		private void RequestCameraPermissionAndroid()
		{
			var callback = new PermissionCallbacks();
			// 카메라 권한 얻었을 때
			callback.PermissionGranted += (_) => CameraPermissionGranted();
			// 카메라 권한 못 얻었을 때
			callback.PermissionDeniedAndDontAskAgain += (_) => _settingsButton.gameObject.SetActive(true);

			Permission.RequestUserPermission(Permission.Camera, callback);
		}
		#elif UNITY_IOS
		private IEnumerator RequestCameraPermissionIOS()
		{
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
			if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
			{
				// 카메라 권한 못 얻음
				if (++_failedCount >= 2)
				{
					_settingsButton.gameObject.SetActive(true);
				}
				yield break;
			}

			// 카메라 권한 얻음
			CameraPermissionGranted();
		}
		#endif

		public void OpenPermissionSettings()
		{
			#if UNITY_ANDROID
			using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			using var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			/*
			// toast message 왠진 몰라도 잘 안 됨
			string toastMessage = "개인정보 보호 - 권한에서 카메라 권한을 허용해주세요.";

			using (var toastClass = new AndroidJavaClass("android.widget.Toast"))
			{
				using var context = unityActivity.Call<AndroidJavaObject>("getApplicationContext");
				var duration = toastClass.GetStatic<int>("LENGTH_LONG");
				unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
				{
					using var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", context, toastMessage, duration);
					toastObject.Call("show");
				}));
			}
			*/

			// open app settings
			string packageName = unityActivity.Call<string>("getPackageName");

			using (var uriClass = new AndroidJavaClass("android.net.Uri"))
			using (var uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
			using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
			{
				intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
				intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
				unityActivity.Call("startActivity", intentObject);
			}
			#elif UNITY_IOS
			// TODO: IOS Support
			#endif
		}
	}
}
