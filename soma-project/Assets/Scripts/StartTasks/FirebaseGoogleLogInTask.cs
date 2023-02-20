using System;
using System.Collections;
using UnityEngine;
using SOMA.Managers;

namespace SOMA.StartTasks
{
	public class FirebaseGoogleLogInTask : StartTask
	{
		[SerializeField]
		private FirebaseLogInManager _firebaseLogInScript;

		private IEnumerator Start() 
		{
			Debug.Log("로그인 될때까지 대기");
			yield return new WaitUntil(() =>  _firebaseLogInScript.IsLogInSuccess);
			Debug.Log("로그인 끝");
			FinishTask();	
		}
	}	
}
