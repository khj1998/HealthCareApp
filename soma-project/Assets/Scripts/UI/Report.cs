using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using SOMA.Managers;

namespace SOMA.UI
{
	public class Report : MonoBehaviour
	{
		public GameObject ReportObject;

		[SerializeField]
		private TextMeshProUGUI _title;

		[SerializeField]
		private TextMeshProUGUI _totalExerciseTime;

		[SerializeField]
		private TextMeshProUGUI _totalGainExp;

		private Vector3 _initialPosition;

		[SerializeField]
		private TextMeshProUGUI _upperGainText;

		[SerializeField]
		private TextMeshProUGUI _coreGainText;

		[SerializeField]
		private TextMeshProUGUI _lowerGainText;

		private int minutes;
		private int seconds;
		private int tempTotalTime;

		private void Awake() 
		{
			showReport();
		}

		private void showReport()
		{
			tempTotalTime = (Int32)Math.Round(DataManager.Instance.ReportDataStorage.TotalExerciseTime);
			minutes = tempTotalTime / 60;
			seconds = tempTotalTime - minutes*60;

			_title.text = DataManager.Instance.ReportDataStorage.Title;
			_totalExerciseTime.text = minutes.ToString()+"분 "+seconds.ToString()+"초";
			_totalGainExp.text = DataManager.Instance.ReportDataStorage.TotalGainExp.ToString();
			_upperGainText.text = DataManager.Instance.ReportDataStorage.UpperGainExp.ToString();
			_coreGainText.text = DataManager.Instance.ReportDataStorage.CoreGainExp.ToString();
			_lowerGainText.text =DataManager.Instance.ReportDataStorage.LowerGainExp.ToString();
		}

		public void ChangeSceneToMain()
		{
			ReportObject.SetActive(false);
			SceneManager.LoadScene("MainScene");
		}
	}
}
