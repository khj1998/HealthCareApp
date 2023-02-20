using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using SOMA.Managers;

namespace SOMA.UI
{
	public class DeckPreview : MonoBehaviour
	{
		private const int EXER_TIME_START = 3; // 엑셀에서 뽑아온 문자열에서 운동시간 인덱스 번호 시작점.
		private const int REST_TIME_START = 1; // 엑셀에서 뽑아온 문자열에서 휴식시간 인덱스 번호 시작점.
		private float singleExerCount=0.0f;
		public string DeckName;
		public int DeckCount;
		public int AllSetTime = 0;
		public List<int> SetPartExp = new List<int> { 0, 0, 0 };

		[SerializeField]
		private GameObject _deckPreview;
		[SerializeField]
		private TextMeshProUGUI _deckTitleText;
		[SerializeField]
		private TextMeshProUGUI _expTimeText;
		[SerializeField]
		private TextMeshProUGUI _pointText;
		private TextMeshProUGUI _cardName;
		private TextMeshProUGUI _cardTime;
		[SerializeField]
		private GameObject[] _cards;
		private int _deckCount;

		public int ExerNameStartIdx=2;
		public int ExerTimeStartIdx=3;
		public int SetMinute;
		public int SetSecond;
		private int setExp;
		private const int _maxCardNumber = 26;

		void Update()
		{
			 if (Input.GetKeyDown(KeyCode.Escape))
        	{
				previewEnd();
				DataManager.Instance.previewOn = false;
        	}
		}

		public void ShowDeckPreview()
		{
			DataManager.Instance.previewOn = true;
			GetDeckInfo();
			ActiveCard();
			ShowDeck();
		}

		void GetDeckInfo()
		{
			GameObject Deck_Button = EventSystem.current.currentSelectedGameObject;

			DeckName = Deck_Button.name;
			DataManager.Instance.ReportDataStorage.Title = DeckName;

			WorkOutManager.Deck = ExerSet_CSV_Reader.ExerSet[DeckName]; // 덱이름에 해당하는 세트정보.
			DeckCount = ExerSet_CSV_Reader.ExerSet[DeckName].Count; // deckLen에 덱 길이 전달.
			
			// 덱의 휴식시간 + 운동시간 총 소모량 deckPreview에 넘길것.
			// 덱의 총 운동시간.
			for (int i = EXER_TIME_START; i < DeckCount-3; i += 4)
			{
				AllSetTime += WorkOutManager.Deck[i];
			}

			// 덱의 총 휴식 시간.
			for (int i = REST_TIME_START; i < DeckCount-3; i += 4)
			{
				AllSetTime += WorkOutManager.Deck[i];
			}

			// 단일 운동 숫자확인. 운동 후 경험치를 피드하는데 활용되는 데이터임.
			for (int i=2;i<DeckCount-3;i+=4)
			{
				singleExerCount +=1.0f;
			}

			DataManager.Instance.SingleExerCount = singleExerCount;
			singleExerCount=0.0f;

			for (int i = DeckCount-3; i < DeckCount; i ++)
			{
				setExp = WorkOutManager.Deck[i];
				DataManager.Instance.EveryPartExp[i-(DeckCount-3)]=setExp;
				SetPartExp[i-(DeckCount-3)] = setExp;
			}
		}

		void ActiveCard()
		{
			_deckPreview.SetActive(true);
			
			for (int i = 0; i < _cards.Length; ++i)
			{
				_cards[i].SetActive(true);
			}
		}

		void ShowDeck()
		{
			SetMinute = AllSetTime / 60;
			SetSecond = AllSetTime - 60 * SetMinute;

			// 덱 인포메이션
			_deckTitleText.text = DeckName; // 덱의 이름.
			if (SetMinute > 0)
			{
				//_expTimeText.text = SetMinute + ":" + SetSecond;
				_expTimeText.text = string.Format("{0:D2}", SetMinute) + " : " + string.Format("{0:D2}",  SetSecond);
			}
			else
			{
				//_expTimeText.text = SetSecond + "s"; // 덱의 총 운동시간.
				_expTimeText.text = "00 : " + string.Format("{0:D2}",  SetSecond);
			}
			_pointText.text = "상체  +" + (SetPartExp[0]) + "\n코어  +" + (SetPartExp[1]) + "\n하체  +" + (SetPartExp[2]) + "\n"; //덱의 각 부위별 경험치량.
			
			string deckPreviewCardStr = "deckPreviewCard";
			string exerName = "";
			string isTime = "";
			_deckCount = DeckCount-3;

			int _cardNumber = 0;

			while (ExerNameStartIdx < _deckCount)
			{
				if (_cardNumber > 0)
				{
					deckPreviewCardStr = $"deckPreviewCard ({_cardNumber})"; //각 덱 프리뷰카드 이름.
				}
				
				//swipe한 덱의 각 운동 세트 정보를 deckPreviewCard에 매핑시킨다.
				_cards[_cardNumber] = GameObject.Find(deckPreviewCardStr);
				exerName = ExerReader.ExerDic[WorkOutManager.Deck[ExerNameStartIdx]].Info[1];
				isTime = ExerReader.ExerDic[WorkOutManager.Deck[ExerNameStartIdx]].IsTime;

				if (exerName != "rest")
				{
					_cardName = _cards[_cardNumber].transform.Find("cardName").GetComponent<TextMeshProUGUI>();
					_cardTime = _cards[_cardNumber].transform.Find("cardTime").GetComponent<TextMeshProUGUI>();
					_cardName.text = exerName; // exer_dic의 anim_num 키로 접근하여 운동 이름 가져온다. Deck의 0,2,4...는 운동 이름 or 쉬기.
					
					if (isTime == "no")
					{
						_cardTime.text = "x " + WorkOutManager.Deck[ExerTimeStartIdx];
					}
					else
					{
						_cardTime.text = WorkOutManager.Deck[ExerTimeStartIdx] + " s";
					}
					_cardNumber += 1;
				}

				ExerNameStartIdx += 2;
				ExerTimeStartIdx += 2;
			}

			// 덱의 길이를 넘어가는것은 없는 세트. 안보여주면 그만임.
			for (int j = _cardNumber; j <= _maxCardNumber; j++)
			{
				deckPreviewCardStr = $"deckPreviewCard ({j})";
				_cards[j] = GameObject.Find(deckPreviewCardStr);
				_cards[j]?.SetActive(false);
			}
		}
		
		void previewEnd() 
		{
			AllSetTime=0; 

			for (int i=0;i<SetPartExp.Count;i++)
			{
				SetPartExp[i]=0;
			}
			ExerNameStartIdx=0;
			ExerTimeStartIdx=1;

			_deckPreview.SetActive(false);
			DataManager.Instance.previewOn=false;
		}
	}
}
