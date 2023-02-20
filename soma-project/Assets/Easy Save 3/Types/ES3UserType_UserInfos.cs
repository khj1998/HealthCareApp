using System;
using UnityEngine;
using SOMA.Managers;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Upper", "Core", "Lower", "GraphStartIdx", "GraphEndIdx", "GraphDatas", "SelectedModel")]
	public class ES3UserType_UserInfos : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_UserInfos() : base(typeof(UserInfos)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (UserInfos)obj;
			
			writer.WriteProperty("Upper", instance.Upper, ES3Type_int.Instance);
			writer.WriteProperty("Core", instance.Core, ES3Type_int.Instance);
			writer.WriteProperty("Lower", instance.Lower, ES3Type_int.Instance);
			writer.WriteProperty("GraphStartIdx", instance.GraphStartIdx, ES3Type_int.Instance);
			writer.WriteProperty("GraphEndIdx", instance.GraphEndIdx, ES3Type_int.Instance);
			writer.WriteProperty("GraphDatas", instance.GraphDatas, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int32>)));
			writer.WriteProperty("SelectedModel", instance.SelectedModel, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (UserInfos)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Upper":
						instance.Upper = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Core":
						instance.Core = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Lower":
						instance.Lower = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "GraphStartIdx":
						instance.GraphStartIdx = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "GraphEndIdx":
						instance.GraphEndIdx = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "GraphDatas":
						instance.GraphDatas = reader.Read<System.Collections.Generic.List<System.Int32>>();
						break;
					case "SelectedModel":
						instance.SelectedModel = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new UserInfos();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_UserInfosArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_UserInfosArray() : base(typeof(UserInfos[]), ES3UserType_UserInfos.Instance)
		{
			Instance = this;
		}
	}
}