using System;
using UnityEngine;
using SOMA.Managers;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("UpperBlender", "CoreBlender", "LowerBlender")]
	public class ES3UserType_UserBlender : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_UserBlender() : base(typeof(UserBlender)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (UserBlender)obj;
			
			writer.WriteProperty("UpperBlender", instance.UpperBlender, ES3Type_int.Instance);
			writer.WriteProperty("CoreBlender", instance.CoreBlender, ES3Type_int.Instance);
			writer.WriteProperty("LowerBlender", instance.LowerBlender, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (UserBlender)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "UpperBlender":
						instance.UpperBlender = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "CoreBlender":
						instance.CoreBlender = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "LowerBlender":
						instance.LowerBlender = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new UserBlender();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_UserBlenderArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_UserBlenderArray() : base(typeof(UserBlender[]), ES3UserType_UserBlender.Instance)
		{
			Instance = this;
		}
	}
}