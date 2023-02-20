using System.Collections;
using UnityEngine;
using SOMA.Managers;

namespace SOMA.StartTasks
{
	public class FirebaseTask : StartTask
	{
		private IEnumerator Start()
		{
			yield return new WaitUntil(() => FirebaseManager.Instance.IsInitialized);
			Debug.Log("파이어베이스 준비됨");
			FinishTask();
		}
	}
}
