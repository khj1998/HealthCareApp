using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.Extensions;
using Google;
using CodeStage.AntiCheat.ObscuredTypes;
using SOMA.StartTasks;
using SOMA.firebaseDB;

namespace SOMA.Managers
{
    public class FirebaseLogInManager : MonoBehaviour
    {
		[SerializeField]
		private GameObject _googleSignInButton;
        // Its required for google signin and we can find this key from firebase here
		private ObscuredString GoogleWebAPI = "210886890732-4pdf3nqeq6r9kog0c464c9um6c9k4gua.apps.googleusercontent.com";

		private GoogleSignInConfiguration configuration;
		Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
		Firebase.Auth.FirebaseUser user;
		public bool IsLogInSuccess;

		private void Awake()
		{
			IsLogInSuccess = false; 
			
			configuration = new GoogleSignInConfiguration
			{
				WebClientId = GoogleWebAPI,
				RequestIdToken = true
			};
		}

		private IEnumerator Start()
		{
			yield return new WaitUntil(() => FirebaseManager.Instance.IsInitialized && FirebaseManager.Instance.IsFirebaseAvaliable);
			yield return new WaitUntil(() => DataManager.Instance.IsPermissionGranted);

			Debug.Log("파이어베이스 초기화 후 로그인 진행");

			GoogleSignInClick();
			#if UNITY_EDITOR
			FirebaseManager.Instance.LoginFinish = true;
			#endif
			
			
			Debug.Log("버전 1.5 아날리틱스 테스트");
			// 로그아웃후 재접했을때
			yield return new WaitUntil(()=>(FirebaseManager.Instance.LoginFinish && FirebaseManager.Instance.ChangeAccount));

			#if UNITY_EDITOR
			Debug.Log("");
			#elif UNITY_ANDROID
			ConnectFirebaseDB.Instance.CheckIdExist(user.UserId);
			#endif

			FirebaseManager.Instance.ChangeAccount = false;
			FirebaseManager.Instance.LoginFinish = false;
		}

		// This method used by LogIn Button in StartScene
		public void GoogleSignInClick()
		{
			if (!FirebaseManager.Instance.ChangeAccount)
			{
				GoogleSignIn.Configuration = configuration;
				GoogleSignIn.Configuration.UseGameSignIn = false;
				GoogleSignIn.Configuration.RequestIdToken = true;
				GoogleSignIn.Configuration.RequestEmail = true;
			}

			#if UNITY_EDITOR
			IsLogInSuccess = true;
			#elif UNITY_ANDROID
			GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
			#endif
		}

		void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
		{
			Debug.Log("로그인 가능여부를 체크합니다.");

			if (task.IsFaulted)
			{
				Debug.LogError("Google Authenticated Fault");
				Debug.LogError(task.Exception.Message);
				_googleSignInButton.SetActive(true);
			}
			else if (task.IsCanceled)
			{
				Debug.LogError("Google Authenticated Task Canceled");
				_googleSignInButton.SetActive(true);
			}
			else
			{
				Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken,null);

				FirebaseManager.Instance._auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
				{
					if (task.IsCanceled)
					{
						Debug.LogError("SignInWithCredentialAsync was canceled");
						return;
					}
					if (task.IsFaulted)
					{
						Debug.LogError("SignInWithCredentialAsync encountered an error: "+task.Exception);
						return;
					}

					user = FirebaseManager.Instance._auth.CurrentUser;
					FirebaseManager.Instance.IdToken = user.UserId;
					FirebaseManager.Instance.LoginFinish = true;
					IsLogInSuccess = true;
					_googleSignInButton.SetActive(false);
				});
			}
		}
        
		//protected override void WhenDestroy(){}
    }
}
