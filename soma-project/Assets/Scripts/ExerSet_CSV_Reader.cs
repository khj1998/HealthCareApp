using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SOMA
{
	public class ExerSet_CSV_Reader 
	{
		private const string FILE_PATH = "Resources/exercise_set.csv";
		private const string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
		private const string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
		private static readonly char[] TRIM_CHARS = { '\"' };
		static private List<int> _exerInfo;
		static public Dictionary<string,List<int>> ExerSet = new Dictionary<string, List<int>>(); 
		static public Dictionary<string,string> SetEnglishNames = new Dictionary<string, string>();

		static public void ExerSet_Read()
		{
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

			for (int i = 0; i < lines.Length-18; i++)
			{
				string[] values = Regex.Split(lines[i], ",");
				_exerInfo = new List<int>();
				ExerSet.Add(values[0],_exerInfo);

				for (int j = 1; j < values.Length; j++)
				{
					int a;
					int.TryParse(values[j], out a);
					ExerSet[values[0]].Add(a);
				}
			}

			for (int i = lines.Length-18; i <lines.Length; i++)
			{
				string[] values = Regex.Split(lines[i], ",");
				SetEnglishNames[values[0]] = values[1];
			}
		}
	}
}
