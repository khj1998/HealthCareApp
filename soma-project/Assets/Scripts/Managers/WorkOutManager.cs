using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SOMA.UI;
using SOMA.Sound;
using System;
using SOMA.Managers;

namespace SOMA
{
    public class WorkOutManager : MonoBehaviour
    {
        [SerializeField]
        private UIOnOffManager uiOnOffManager;

		//소리 관련 클래스
		[SerializeField]
		private TipSoundManager ttsTipSound;
		[SerializeField]
		private StateSound stateSound;
		[SerializeField]
		private Bar restBar;
		[SerializeField]
		private GameObject modelGenerator;
		[SerializeField]
		private Bar inWorkOutBar;

		//다른 오브젝트가 접근하는 변수
		public TextMeshProUGUI TopText;
        public TextMeshProUGUI BottomText;
		public Animator ModelAnim;
		public GameObject Bar;

		[SerializeField]
        public static float Gauge;
        private int aniNum;
		private float clearExerCount;
		private const float _zeroRate = 0.0f;
        public static int BarTime;
        double ExerTime;

        public static List<int> Deck;
		int DeckIndex;
        int DeckLen;
		bool updateOnlyOne;
		float achievenmentRate;
		private bool _isCleared;

        public enum State
        {
            setting,  
            inWorkout,
			outWorkout,
            report,
            waitState // 특정 state에서 코드를 한번만 실행하기 위해 사용
        };

        public static State state; 

		void Awake()
		{
			//일반 운동 씬에서도 일단 배너 파괴
			//SOMA.Ads.BannerAds.Instance.DestroyBanner();
			state = State.setting;
			Gauge = 0;
			aniNum = 0;
			ExerTime = 0;
			clearExerCount = 0.0f;
			DataManager.Instance.previewOn=false;
			_isCleared = false;
		} 
		void Start()
		{
			ModelAnim = modelGenerator.transform.GetChild(0).GetComponent<Animator>();
			//modelGenerator에서 생성한 모델에 접근
		}
        void Update()
        {
            if (state == State.setting)  // 변경씬에서 작동
            {
				Bar.SetActive(true);
				updateOnlyOne = true;
                DeckLen = Deck.Count-3;

				if (Deck[DeckIndex]== 0)
				{
					stateSound.BreakTimePlay(); 
					restTime();
				}

				if( Deck[DeckIndex]!= 0) 
				{
					ttsTipSound.TipPlay(Deck[DeckIndex] - 1); //운동 팁 플레이
					exerciseTime();
				}

				inWorkOutBar.maxGauge = Deck[DeckIndex + 1];
				restBar.maxGauge = Deck[DeckIndex + 1];
				state = State.inWorkout;
				Gauge = 0; //게이지를 0으로 먼저 바꾸고 UI를 변경하면 바 UI가 갑자기 0값으로 바뀌고 사라져서 어색함 => 이렇게 안 하면 게이지가 깜빡임
            }
            else if (state == State.inWorkout)
            {
				inWorkOutBar.gauge = Gauge;
				restBar.gauge = Gauge;
				ExerTime +=Time.deltaTime;
                Gauge += Time.deltaTime;
				BottomText.text = $"{Deck[DeckIndex + 1] - (int)Gauge}";

				bool playSoundOnce = false;
				if (Gauge + Mathf.Epsilon >= Deck[DeckIndex + 1] - 4 && Gauge + Mathf.Epsilon <= Deck[DeckIndex + 1] -3 && Deck[DeckIndex] == 0) //쉬는시간에서 3초 남았을때
				{
					
					if (playSoundOnce == false)
					{
						stateSound.BeepPlay();
						playSoundOnce = true;
					}
				}

				if (Gauge + Mathf.Epsilon >= Deck[DeckIndex+1]) //운동시간 or 휴식시간이 끝났을때
				{
					// 마지막 운동은 쉬는시간이 없어야함
					if (Deck[DeckIndex] == 0) //쉬는시간이면 바로 넘기기
					{
						state = State.setting;
					}
				    else if ((Deck[DeckIndex]) != 0) //0은 휴식임. 0이 아닌경우 운동진행한것.=>부위별로 경험치피드.
					{
						state = State.outWorkout;

						clearExerCount +=1.0f;
					}

					if (DeckIndex + 1 == DeckLen - 1) // 마지막 휴식시간은 굳이 안해도된다.
					{
						_isCleared = true;
						state = State.report;
					}
					else
					{
						 DeckIndex += 2; // 두칸 건너뛰어야 다음 운동 애니메이션 or 쉬는 시간 인덱스로
					}

				}
            }

			else if (state == State.outWorkout)
            {
				uiOnOffManager.setCanNextExercise();
				state = State.waitState; //UI를 한번만 키고 기다리기 , 버튼에서 State.setting로 이동
			}

            else if (state == State.report)
            {
				FillReportStorage();
				ModelAnim.SetInteger("pose",0);

				if (updateOnlyOne)
				{
					updateOnlyOne = false;

					#if !UNITY_EDITOR
					Debug.Log("운동끝 아날리틱스");
					FirebaseManager.Instance.LogEvent("workout_finished",new Firebase.Analytics.Parameter[]{
					new Firebase.Analytics.Parameter("is_cleared", _isCleared ? "yes" : "no"),
					new Firebase.Analytics.Parameter("exercise_time",ExerTime)
					});
					#endif
					
					DataManager.Instance.GetGraphData(achievenmentRate);
					DataManager.Instance.UpdateDataAfterExer(achievenmentRate);
				}
				
				uiOnOffManager.TurnOnReportCanvas();
				DataManager.Instance.InitDataInfos();
            }
        }

		void restTime()
		{
			string nextExerName = ExerReader.ExerDic[Deck[DeckIndex+2]].Name;
			string isTime = ExerReader.ExerDic[Deck[DeckIndex+2]].IsTime;

			if (isTime=="no") 
			{
				TopText.text = $"휴식\n다음: {nextExerName}  x{Deck[DeckIndex+3]} ";
			}
			else  
			{
				TopText.text = $"휴식\n다음: { nextExerName} {Deck[DeckIndex+3]}s";
			}
					
			if (uiOnOffManager!=null)
			{
				uiOnOffManager.setRest();
			}

            aniNum = Deck[DeckIndex+2];
			ModelAnim.SetInteger("pose",aniNum);
		}

		void exerciseTime()
		{
			string nowExerName = ExerReader.ExerDic[Deck[DeckIndex]].Name;  
			string isTime = ExerReader.ExerDic[Deck[DeckIndex]].IsTime;    

			if (isTime=="no") 
			{
				TopText.text = $"{nowExerName}  x{Deck[DeckIndex+1]}";  
                        
			}
			else  
			{
				TopText.text = $"{nowExerName}  {Deck[DeckIndex+1]}s";  
			}

			if (uiOnOffManager!=null)
			{
				uiOnOffManager.setWorkout();
			}

            aniNum = Deck[DeckIndex];
			ModelAnim.SetInteger("pose",aniNum);
		}

		public void FillReportStorage()
		{
			int totalExp=0;
			
			if (clearExerCount > 0.0f)
			{
				achievenmentRate = clearExerCount/(DataManager.Instance.SingleExerCount);
			}
			else
			{
				achievenmentRate = _zeroRate;
			}


			DataManager.Instance.ReportDataStorage.TotalExerciseTime = ExerTime;
			DataManager.Instance.ReportDataStorage.UpperGainExp = (int)Math.Ceiling(DataManager.Instance.EveryPartExp[0]*achievenmentRate);
			DataManager.Instance.ReportDataStorage.CoreGainExp = (int)Math.Ceiling(DataManager.Instance.EveryPartExp[1]*achievenmentRate);
			DataManager.Instance.ReportDataStorage.LowerGainExp = (int)Math.Ceiling(DataManager.Instance.EveryPartExp[2]*achievenmentRate);

			for (int i=0;i<3;i++)
			{
				totalExp += (int)Math.Ceiling(DataManager.Instance.EveryPartExp[i]*achievenmentRate);
			}

			DataManager.Instance.ReportDataStorage.TotalGainExp =totalExp;
		}

		public void addGauge()
		{
			//Gauge = Deck[DeckIndex + 1];
			Gauge += 10;
		}
		public void OnClickNextBtn()
		{
			state = State.setting;
		}
    }
}
