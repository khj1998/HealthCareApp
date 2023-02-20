using UnityEngine;

namespace SOMA
{
	public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
	{
		private static T _instance;

		public static T Instance
		{
			get {
				if (_instance is null)
				{
					_instance = FindObjectOfType<T>();
					if (_instance is null)
					{
						_instance = new GameObject(typeof(T).Name).AddComponent<T>();
					}
				}
				return _instance;
			}
		}

		protected void Awake()
		{
			if (_instance != null && _instance != this)
			{
				Debug.LogError($"An instance of {typeof(T).Name} singleton already exists.");
				Destroy(this.gameObject);
				return;
			}
			else
			{
				_instance = (T)this;
			}
			DontDestroyOnLoad(gameObject);

			WhenAwake();
		}

		protected abstract void WhenAwake();

		protected void OnDestroy()
		{
			WhenDestroy();

			_instance = null;
		}

		protected abstract void WhenDestroy();
	}
}
