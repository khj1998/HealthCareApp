using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace SOMA
{
	[Serializable]
	public class Item
	{
		public string Name;
		public string[] Info;
		public string IsTime;

		public Item(string[] row)
		{
			Info = row;
			Name = row[1];
			IsTime = row[row.Length-1];
		}
	}

	public class ExerReader 
	{
		private const string FILE_PATH = "Resources/exercise.csv";
		private const string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
		private const string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
		private static readonly char[] TRIM_CHARS = { '\"' };

		static public List<Item> ExerInfo;
		static public Dictionary<int, Item> ExerDic = new Dictionary<int, Item>();

		public static void Card_Read()
		{
			ExerInfo = new List<Item>();
			SOMA.AdressablesUtility.LoadAssetSync<TextAsset>(FILE_PATH, textAsset => ParseCSV(textAsset.text));
		}

		private static void ParseCSV(string csv)
		{
			var lines = Regex.Split(csv, LINE_SPLIT_RE);

			if (lines.Length <= 1)
			{
				Debug.Log("Empty CSV");
				return;
			}

			string[] header = Regex.Split(lines[0], SPLIT_RE);

			for (int i = 1; i < lines.Length ; i++)
			{
				string[] values = Regex.Split(lines[i], ",");
				ExerInfo.Add(new Item(values));
				ExerDic.Add(int.Parse(values[0]), ExerInfo[i - 1]);
			}
			string[] rest_routine = { "쉬는 시간", "rest" };
			ExerDic.Add(0, new Item(rest_routine));
		}
	}
}
