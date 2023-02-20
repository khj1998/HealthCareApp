using System;
using UnityEngine;
using SOMA.Managers;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("SquatBestScore", "PushUpBestScore")]
	public class ES3UserType_ChallengeScores : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ChallengeScores() : base(typeof(ChallengeScores)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (ChallengeScores)obj;
			
			writer.WriteProperty("SquatBestScore", instance.SquatBestScore, ES3Type_int.Instance);
			writer.WriteProperty("PushUpBestScore", instance.PushUpBestScore, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (ChallengeScores)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "SquatBestScore":
						instance.SquatBestScore = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "PushUpBestScore":
						instance.PushUpBestScore = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new ChallengeScores();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ChallengeScoresArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ChallengeScoresArray() : base(typeof(ChallengeScores[]), ES3UserType_ChallengeScores.Instance)
		{
			Instance = this;
		}
	}
}