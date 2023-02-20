using UnityEngine;
using Google;
using UnityEngine.SceneManagement;
using SOMA.firebaseDB;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SOMA.Managers
{
	public class LogOutManager : MonoBehaviour
{
		[SerializeField]
		private GameObject _logOutPanel;

    // Its required for google signin and we can find this key from firebase here
		private ObscuredString GoogleWebAPI = "210886890732-4pdf3nqeq6r9kog0c464c9um6c9k4gua.apps.googleusercontent.com";

		private GoogleSignInConfiguration configuration;
		Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
		Firebase.Auth.FirebaseUser user;
        public bool IsReady { get; private set; } = false;

        void Awake()
        {
			configuration = new GoogleSignInConfiguration
			{
				WebClientId = GoogleWebAPI,
				RequestIdToken = true
			};
        }

		public void OnLogOutPanel()
		{
			_logOutPanel.SetActive(true);
		}

		public void OffLogOutPanel()
		{
			_logOutPanel.SetActive(false);
		}

		public void Continue()
		{
			_logOutPanel.SetActive(false);
		}

		public void GoogleSignOut()
		{
			#if UNITY_EDITOR
			Debug.Log("");
			#elif UNITY_ANDROID
			ConnectFirebaseDB.Instance.initBackUpList();
            GoogleSignIn.DefaultInstance.SignOut();
			#endif
			
			FirebaseManager.Instance.ChangeAccount = true;
			SceneManager.LoadScene("StartScene");
			Debug.Log("로그아웃을 진행합니다.");
		}
}
}