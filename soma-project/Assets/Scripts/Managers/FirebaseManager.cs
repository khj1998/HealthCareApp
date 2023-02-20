using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using Google;
using TMPro;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SOMA.Managers
{
	[Serializable]
	public class FirebaseManager : MonoBehaviourSingleton<FirebaseManager>
	{
		private static readonly Regex NAME_REGEX = new Regex(@"^[a-z][a-z0-9_]*$", RegexOptions.Compiled);

		public FirebaseApp _app = null;
		public FirebaseAuth _auth = null;
		public ObscuredString IdToken { get; set; }
		public DatabaseReference reference;
		private GoogleSignInConfiguration configuration;
		Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

		public bool IsFirebaseAvaliable { get; private set; } = false;
		public bool IsInitialized { get; private set; } = false;
		public bool ChangeAccount = false;
		public bool LoginFinish = false;

		protected override void WhenAwake()
		{
			FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
				var dependencyStatus = task.Result;
				if (dependencyStatus == DependencyStatus.Available) {
					// Create and hold a reference to your FirebaseApp,
					// where app is a FirebaseApp property of your application class.
					_app = FirebaseApp.DefaultInstance;
					_auth = FirebaseAuth.DefaultInstance;

					// Set a flag here to indicate whether Firebase is ready to use by your app.
					IsFirebaseAvaliable = true;
					Debug.Log("Firebase Initialized!!!");

				} else {
					Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
					// Firebase Unity SDK is not safe to use here.
					IsFirebaseAvaliable = false;
				}

				IsInitialized = true;
			});
		}

		private bool IsValidName(string name, int length)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			if (name.Length > length)
			{
				return false;
			}
			return NAME_REGEX.IsMatch(name);
		}

		private bool IsValidEventName(string name) => IsValidName(name, 32);
		private bool IsValidParameterName(string name) => IsValidName(name, 40);
		private bool IsValidParameterValue(string value) => !string.IsNullOrEmpty(value) && value.Length <= 100;

		public void LogEvent(string name)
		{
			if (!IsValidEventName(name))
			{
				Debug.LogError($"Invalid event name: {name}");
				return;
			}
			if (IsFirebaseAvaliable)
			{
				FirebaseAnalytics.LogEvent(name);
			}
		}

		public void LogEvent(string name, params Parameter[] parameters)
		{
			if (!IsValidEventName(name))
			{
				Debug.LogError($"Invalid event name: {name}");
				return;
			}
			if (IsFirebaseAvaliable)
			{
				FirebaseAnalytics.LogEvent(name, parameters);
			}
		}

		public void LogEvent(string name, string parameterName, int parameterValue)
		{
			if (!IsValidEventName(name))
			{
				Debug.LogError($"Invalid event name: {name}");
				return;
			}
			if (!IsValidParameterName(parameterName))
			{
				Debug.LogError($"Invalid parameter name: {parameterName}");
				return;
			}
			if (IsFirebaseAvaliable)
			{
				FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
			}
		}

		public void LogEvent(string name, string parameterName, long parameterValue)
		{
			if (!IsValidEventName(name))
			{
				Debug.LogError($"Invalid event name: {name}");
				return;
			}
			if (!IsValidParameterName(parameterName))
			{
				Debug.LogError($"Invalid parameter name: {parameterName}");
				return;
			}
			if (IsFirebaseAvaliable)
			{
				FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
			}
		}

		public void LogEvent(string name, string parameterName, double parameterValue)
		{
			if (!IsValidEventName(name))
			{
				Debug.LogError($"Invalid event name: {name}");
				return;
			}
			if (!IsValidParameterName(parameterName))
			{
				Debug.LogError($"Invalid parameter name: {parameterName}");
				return;
			}
			if (IsFirebaseAvaliable)
			{
				FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
			}
		}
		
		public void LogEvent(string name, string parameterName, string parameterValue)
		{
			if (!IsValidEventName(name))
			{
				Debug.LogError($"Invalid event name: {name}");
				return;
			}
			if (!IsValidParameterName(parameterName))
			{
				Debug.LogError($"Invalid parameter name: {parameterName}");
				return;
			}
			if (!IsValidParameterValue(parameterValue))
			{
				Debug.LogError($"Invalid parameter value: {parameterValue}");
				return;
			}
			if (IsFirebaseAvaliable)
			{
				FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
			}
		}

		public void SetUserProperty(string name, string property)
		{
			if (!IsValidParameterName(name))
			{
				Debug.LogError($"Invalid parameter name: {name}");
				return;
			}
			if (!IsValidParameterValue(property))
			{
				Debug.LogError($"Invalid parameter value: {property}");
				return;
			}
			if (IsFirebaseAvaliable)
			{
				FirebaseAnalytics.SetUserProperty(name, property);
			}
		}

		protected override void WhenDestroy() {
			_app?.Dispose();
			_auth?.Dispose();
		}
	}
}
