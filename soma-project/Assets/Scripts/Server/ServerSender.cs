using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;

namespace SOMA.Server
{
	[Serializable]
	public class SendExerData
	{
		public string Token;
		
		public int ExerModeCount;
		public int ChallengeModeCount;

		public int SquatBestScore;
		public int PushUpBestScore;

		public SendExerData()
		{
			Token = "user"; 
			//ExerModeCount = DataManager.Instance.UserInfosTable.ExerModeCount;
			//ChallengeModeCount = DataManager.Instance.UserInfosTable.ChallengeModeCount;
			//SquatBestScore = DataManager.Instance.ChallengeScoresTable.SquatBestScore;
			//PushUpBestScore = DataManager.Instance.ChallengeScoresTable.PushUpBestScore;
		}
	}

	public class ServerSender: MonoBehaviourSingleton<ServerSender>
	{
		private bool IsBackUpEnd = false;

		protected override void WhenAwake() {}

		IEnumerator SendWebData(string address, string ID, string url)
		{
			const string API_KEY = "JVFYJ1B-ESEMJR7-GGV0DWV-40H3FAB/";
			address = address + API_KEY + ID + url;
			
			SendExerData jsonData = new SendExerData();
			string send = JsonUtility.ToJson(jsonData);

			byte[] jsonToSend = new UTF8Encoding().GetBytes(send);

			using (UnityWebRequest post = UnityWebRequest.Post(address, "POST"))
			{
				post.uploadHandler = new UploadHandlerRaw(jsonToSend);
				post.downloadHandler = new DownloadHandlerBuffer();
				post.SetRequestHeader("Content-Type", "application/json");

				yield return post.SendWebRequest();

				if (post.result != UnityWebRequest.Result.Success)
				{
					Debug.LogError(post.error + " error occured!");
					IsBackUpEnd = true;
				}
				else 
				{
					Debug.Log(post.downloadHandler.text);
					IsBackUpEnd = true;
				}
			}
		}

		public void SendingAfterExer()
		{
			if (!IsBackUpEnd)
			{
				StartCoroutine(SendWebData("", "", "/DataBackup"));
			}
		}

		protected override void WhenDestroy() {}
	}
}
