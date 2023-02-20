using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SOMA.StartTasks;

namespace SOMA.Managers
{
	public class StartManager : MonoBehaviour
	{
		[SerializeField]
		private StartTask[] _startTasks;

		private IEnumerator Start()
		{
			yield return new WaitUntil(() => _startTasks.All(x => x.IsFinished));

			if (DataManager.Instance.UserInfosTable.SelectedModel == -1)
			{
				Debug.Log("모델 선택씬");
				SceneManager.LoadScene("SelectModel");
			}
			else
			{
				Debug.Log($"이미 {DataManager.Instance.UserInfosTable.SelectedModel}번 모델 선택함. 메인화면으로");
				SceneManager.LoadScene("MainScene");
			}
		}
	}
}
