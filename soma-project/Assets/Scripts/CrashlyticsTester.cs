using UnityEngine;

public class CrashlyticsTester : MonoBehaviour {

	int updatesBeforeException;

	// Use this for initialization
	void Start () {
		updatesBeforeException = 0;
		// Initialize Firebase
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				// Create and hold a reference to your FirebaseApp,
				// where app is a Firebase.FirebaseApp property of your application class.
				// Crashlytics will use the DefaultInstance, as well;
				// this ensures that Crashlytics is initialized.
				Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

				// Set a flag here for indicating that your project is ready to use Firebase.
			}
			else
			{
				UnityEngine.Debug.LogError(System.String.Format(
				"Could not resolve all Firebase dependencies: {0}",dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});
	}

	// Update is called once per frame
	void Update()
	{
		// Call the exception-throwing method here so that it's run
		// every frame update
		throwExceptionEvery60Updates();
	}

	// A method that tests your Crashlytics implementation by throwing an
	// exception every 60 frame updates. You should see non-fatal errors in the
	// Firebase console a few minutes after running your app with this method.
	void throwExceptionEvery60Updates()
	{
		if (updatesBeforeException > 0)
		{
			updatesBeforeException--;
		}
		else
		{
			// Set the counter to 60 updates
			updatesBeforeException = 60;

			// Throw an exception to test your Crashlytics implementation
			throw new System.Exception("test exception please ignore");
		}
	}
}
