using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SOMA.Server
{
	public class ServerTalker : MonoBehaviour
	{
		private readonly char[] DELIMITER_CHARS = {',','[',']'};
		static public FirstData ReceivedData;
		private bool serverFirstConnect;

		[Serializable]
		public class FirstData
		{
			public string Token;
		
			public int ExerModeCount;
			public int ChallengeModeCount;

			public int SquatBestScore;
			public int PushUpBestScore;
		}

		void Start() 
		{
			serverFirstConnect = false;
			
			if (!serverFirstConnect)
			{
				TalkFirst();
			}
		}

		IEnumerator GetWebData(string address, string ID)
		{
			const string API_KEY = "JVFYJ1B-ESEMJR7-GGV0DWV-40H3FAB/";
			address = address + API_KEY + ID;

			using (UnityWebRequest get = UnityWebRequest.Get(address))
			{
				yield return get.SendWebRequest();

				if (get.result != UnityWebRequest.Result.Success)
				{
					serverFirstConnect=true;
					//Debug.LogError(web.error + " error occured!");
				}
				else
				{
					serverFirstConnect=true;
					Debug.Log(get.downloadHandler.text);
				}
			}
		}

		public void TalkFirst()
		{
			if (!serverFirstConnect)
			{
				StartCoroutine(GetWebData("", ""));
			}
		}
	}
}
