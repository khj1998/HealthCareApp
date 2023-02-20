using System;
using UnityEngine;
using SOMA.Managers;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("UpperLog", "CoreLog", "LowerLog", "LastExerLog")]
	public class ES3UserType_UserLog : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_UserLog() : base(typeof(UserLog)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (UserLog)obj;
			
			writer.WriteProperty("UpperLog", instance.UpperLog, ES3Type_int.Instance);
			writer.WriteProperty("CoreLog", instance.CoreLog, ES3Type_int.Instance);
			writer.WriteProperty("LowerLog", instance.LowerLog, ES3Type_int.Instance);
			writer.WriteProperty("LastExerLog", instance.LastExerLog, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (UserLog)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "UpperLog":
						instance.UpperLog = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "CoreLog":
						instance.CoreLog = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "LowerLog":
						instance.LowerLog = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "LastExerLog":
						instance.LastExerLog = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new UserLog();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_UserLogArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_UserLogArray() : base(typeof(UserLog[]), ES3UserType_UserLog.Instance)
		{
			Instance = this;
		}
	}
}