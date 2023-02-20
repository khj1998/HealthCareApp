using System;
using UnityEngine;
using SOMA.Managers;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("NewCharacter", "RemoveBanner")]
	public class ES3UserType_InAppProducts : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_InAppProducts() : base(typeof(InAppProducts)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (InAppProducts)obj;
			
			writer.WriteProperty("NewCharacter", instance.NewCharacter, ES3Type_bool.Instance);
			writer.WriteProperty("RemoveBanner", instance.RemoveBanner, ES3Type_bool.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (InAppProducts)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "NewCharacter":
						instance.NewCharacter = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "RemoveBanner":
						instance.RemoveBanner = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new InAppProducts();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_InAppProductsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_InAppProductsArray() : base(typeof(InAppProducts[]), ES3UserType_InAppProducts.Instance)
		{
			Instance = this;
		}
	}
}