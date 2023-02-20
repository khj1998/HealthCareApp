using UnityEngine;
using UnityEngine.SceneManagement;
using SOMA.Managers;

namespace SOMA.UI
{
	public class DeckBtnManager : MonoBehaviour
	{
		public static string CardName;
		private string _englishCardNames;
		[SerializeField]
		private GameObject _deckPreview;
		[SerializeField]
		private DeckPreview _deckPreviewScript;

		public void deckPreviewToExercise() // 누르면 운동시작
		{
			CardName = _deckPreviewScript.DeckName; 
			_englishCardNames = ExerSet_CSV_Reader.SetEnglishNames[CardName];
			Debug.Log(_englishCardNames);
			WorkOutManager.Deck = ExerSet_CSV_Reader.ExerSet[CardName];
			
			_deckPreviewScript.AllSetTime=0;
			for (int i=0;i<_deckPreviewScript.SetPartExp.Count;i++)
			{
				_deckPreviewScript.SetPartExp[i]=0;
			}
			_deckPreviewScript.ExerNameStartIdx=0;
			_deckPreviewScript.ExerTimeStartIdx=1;

			#if !UNITY_EDITOR
			Debug.Log("선택덱 아날리틱스");
			FirebaseManager.Instance.LogEvent($"deckname_{_englishCardNames}",_englishCardNames,"clicked");
			#endif

			SceneManager.LoadScene("WorkOut");
			_deckPreview.SetActive(false);
		}

		public void StartChallenge()
		{
			SceneManager.LoadScene("Challenge");
		}

		public void deckPreviewEnd() // 누르면 프리뷰 꺼짐 
		{
			_deckPreviewScript.AllSetTime=0; 

			for (int i=0;i<_deckPreviewScript.SetPartExp.Count;i++)
			{
				_deckPreviewScript.SetPartExp[i]=0;
			}
			_deckPreviewScript.ExerNameStartIdx=0;
			_deckPreviewScript.ExerTimeStartIdx=1;

			_deckPreview.SetActive(false);
		}
	}
}
