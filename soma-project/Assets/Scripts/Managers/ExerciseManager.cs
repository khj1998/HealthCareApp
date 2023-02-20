using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.UI;
using SOMA.MediaPipe;
using SOMA.MediaPipe.Classification;
using SOMA.Model;
using SOMA.Server;
using SOMA.Sound;
using SOMA.UI;
using Mediapipe;
using TMPro;

/*namespace SOMA
{
	[System.Serializable]
	public class Serialization<T>
	{
		public Serialization(List<T> _target) => Exercise = _target;
		public List<T> Exercise;
	}

	public class ExerciseManager : MonoBehaviour
	{
		[SerializeField]
		private BlendShape _blendShape;
		[SerializeField]
		private TextMeshProUGUI _chatText;

		static public bool IsDailyQuest; // 데일리 퀘스트인지 tag를 통해서 확인
		static public int stage = 0;
		static public float gauge = 0;
		double ExerTime = 0;
		public GameObject deckViewCanvas;
		static public string card_name; // 운동 카드 이름 ex) lower,lower2,lower3
		public GameObject bar;
		public GameObject waitBar;
		public GameObject swipeBar;
		public GameObject moreExerciseBtn;
		public Animator model_ani;
		public ScrollRect mainScrollView;
		public GameObject exerciseReport;
		public GameObject deckPreview;
		public GameObject backBtn;
		public GameObject mainContent;
		public GameObject rllyFinish;
		public GameObject darkBlur;
		[SerializeField]
		private DataDB DataDB;
		[SerializeField]
		private ServerTalker ServerTalker;
		[SerializeField]
		private ServerSender ServerSender;
		public GameObject TotalExpUI;
		public TipSoundManager tipSound;
		public StateSound stateSnd;
		public PoseSolution Solution;
		public PoseClassifierMap PoseClassifierPrefabs;
		private PoseClassifierProcessor _poseClassifierPrefab;
		private bool _autoGauge;
		static public int now_rest_time; // 현재 운동 진행후 휴식시간. 게이지 바 비율 때문에 존재

		//content 움직이는 변수들
		bool isMoveContent = false;
		float ContentYPos = 0;
		int next_anim_num; // 휴식시간에 보여줄 다음 운동 애니메이션 번호
		string FILEPATH;

		// 아래는 운동 세트 관련
		static public List<int> Deck; // 운동 부위에 해당하는 덱 정보 문자열을 swipe카드 실행시 받아온다.
		int DeckIndex = 0;
		// 경험치 피드 결과 찍는데 필요한 리스트
		public static List<string> parts = new List<string> { "상체", "코어", "하체" };
		static public List<int> last_exp = new List<int>(); // 피드되기 전의 부위별 경험치
		static public Dictionary<string,int> FindLack = new Dictionary<string, int>(); //부족한 부위를 찾기위한 딕셔너리.
		static public List<int> every_set_exp = new List<int>(); // 세트가 끝났을 때 각 부위별 얻은 경험치량 저장.
		static public int bar_time = 0;
		int Len;
		static public List<int> BlenderValue = new List<int>(); // upper, core, lower순으로 통일
		int UpdateTime;
		int DayValue;
		int JustOneRead=0;
		bool DailyQuestClear;
		static public bool DataSend;

		private void Awake()
		{
			DataSend = true;
			IsDailyQuest = false;
			DayValue = DateTime.Now.Minute; // 매일 매일 세트 추천이 달라져야함. 일 수 %30 을 나눈 나머지값으로 인덱스를 계산해 추천한다.
			FILEPATH = Application.persistentDataPath + "/exercise";
			ExerReader.Card_Read(); // 개별 운동 카드 정보 불러오기
			ExerSet_CSV_Reader.ExerSet_Read(); // 운동 세트 정보 불러오기
			//ExerSet_CSV_Reader.CalTimeSum(); // 운동 세트별로 총 세트 진행시간,소모칼로리 정보 exer_set List<int> value에 추가.
		}

		private void Update()
		{
			if (Deck!=null) //덱을 swipe했을때만 Deck에 리스트정보가 채워짐.
			{
				Len=Deck.Count;
			}
			// 부족한 부위 추천 처음 앱켰을때 + 세트진행후 부족한부위 업뎃정보.해당 정보를 바탕으로 운동세트 추천예정.
			FindLack=FindLack.OrderBy(x=>x.Value).ToDictionary(x=>x.Key,x=>x.Value); //부족한 부위 찍어내기위한 정렬
			var Lack=FindLack.ToList();
			// 만약 각 부위 경험치량이 모두 같다면 운동세트를 랜덤하게 추천해주기
			if (JustOneRead==0)
			{
				Debug.Log("stage0 경험치: "+last_exp[0]+" "+last_exp[1]+" "+last_exp[2]);
				JustOneRead+=1;
				/*if (Lack[0].Value==Lack[Lack.Count-1].Value)
				{
					if (Lack[0].Value==0) _chatText.text="반가워!\n모든 부위에 운동을 진행하지 않았어!";
					else _chatText.text="반가워!\n모든 부위 경험치가 동일해!";
				}
				else
				{
					_chatText.text=string.Format("반가워!\n {0} 경험치가 가장 부족해!",Lack[0].Key);
				}
			}

			if (DataSend) 
			{
				if (ServerTalker.IsConnect() && DataSend)
				{
					// Awake에서 읽었는데 다시 읽는 이유? 만약 세트가 끝나서 stage0으로 돌아오면, 업뎃된 데이터들을 읽어서 백업해주어야하기 때문이다. 
					DataDB.DB_Read(string.Format("SELECT lower,upper,core,quest_num,quest_log FROM user_info WHERE user_id=\"{0}\"","user"),"user_info"); 
					DataDB.DB_Read(string.Format("SELECT lower_blender,upper_blender,core_blender FROM user_blender WHERE user_id=\"{0}\"","user"),"user_blender");
					DataDB.DB_Read(string.Format("SELECT lower_log,upper_log,core_log,lower_loss,upper_loss,core_loss FROM user_log WHERE user_id=\"{0}\"","user"),"user_log");

					ServerSender.ExpBlender.Add(DataDB.UserInfos[0].Upper);
					ServerSender.ExpBlender.Add(DataDB.UserInfos[0].Core);
					ServerSender.ExpBlender.Add(DataDB.UserInfos[0].Lower);

					for (int i=0;i<DataDB.UserBlender.Count;i++)
					{
						ServerSender.ExpBlender.Add(DataDB.UserBlender[i]);
					}

					ServerSender.ExpBlender.Add(DataDB.UserInfos[0].QuestNum);

					/*
					for (int i=0;i<DataDB.user_log.Count;i++)
					{
						ServerSender.UserLog.Add(DataDB.user_log[i]);
					}
					

					ServerSender.QuestLog=DataDB.UserInfos[0].QuestLog;
					ServerSender.Sending();

					DataSend=false;
				}
			}
			
			if (stage == 1)
			{
				deckViewCanvas.SetActive(false);
				moveContent(0);
				moreExerciseBtn.SetActive(false);
				backBtn.SetActive(true);
				swipeBar.SetActive(false); //덱 실행 시 스와이프 바 제거
				mainScrollView.vertical = false; // 운동 시작하면 스크롤 못 함
				string chat;

				if (Deck[DeckIndex]== 0 && JustOneRead==1)
				{
					stateSnd.BreakTimePlay();
					JustOneRead+=1;
					string ExerEngName=ExerReader.ExerDic[Deck[DeckIndex+2].ToString()].Name;
					string is_time=ExerReader.ExerDic[Deck[DeckIndex+2].ToString()].IsTime;

					/*if (is_time=="no") chat = "<size=100>휴식\n다음: </size>"+ExerciseDB.EngKorName[ExerEngName]+" "+Deck[DeckIndex+1]+"회";
					else  chat = "휴식\n다음: "+ExerciseDB.EngKorName[ExerEngName]+"\n"+Deck[DeckIndex+1]+"초";

					if (is_time=="no") 
					{
						chat = "\n\n"+$"<size=70>휴식: {Deck[DeckIndex+1]}초\n\n다음: {ExerciseDB.EngKorName[ExerEngName]} {Deck[DeckIndex+3]} 회</size>";
					}
					else  
					{
						chat = "\n\n"+$"<size=70>휴식: {Deck[DeckIndex+1]}초\n\n다음: {ExerciseDB.EngKorName[ExerEngName]} {Deck[DeckIndex+3]} 초</size>";
					}

					_chatText.text = chat;
					model_ani.SetInteger("pose",Deck[DeckIndex+2]);
					waitBar.SetActive(true);

					_autoGauge = true;
				}

				if( Deck[DeckIndex]!=0 && JustOneRead==1 ) // 운동 애니메이션일때.
				{
					JustOneRead+=1;
					tipSound.TipPlay(Deck[DeckIndex]);
					string ExerEngName=ExerReader.ExerDic[Deck[DeckIndex].ToString()].Name;
					string is_time=ExerReader.ExerDic[Deck[DeckIndex].ToString()].IsTime;

					if (is_time=="no") 
					{
						chat = $"{ExerciseDB.EngKorName[ExerEngName]} \n\n <size=150>{Deck[DeckIndex+1]} 회</size>";
					}
					else  
					{
						chat = $"{ExerciseDB.EngKorName[ExerEngName]} \n\n <size=150>{Deck[DeckIndex+1]} 초</size>";
					}

					_chatText.text = chat;
					model_ani.SetInteger("pose", Deck[DeckIndex]);
					bar.SetActive(true);

					/*var poseClassifierPrefab = PoseClassifierPrefabs[Deck[DeckIndex]];
					if (poseClassifierPrefab is not null)
					{
						_poseClassifierPrefab = Instantiate(poseClassifierPrefab, this.transform);
						Solution.Callback = (landmarks) => gauge = _poseClassifierPrefab.Classify(landmarks);
						_autoGauge = false;
					}
					else
					{
						Debug.LogWarning($"{Deck[DeckIndex]} - {ExerEngName} classifier is missing!!!");
						_autoGauge = true;
					}
				}
				stage = 2;
			}
			else if (stage == 2)
			{
				if (_autoGauge)
				{
					gauge += Time.deltaTime;
				}
				bar_time = Deck[DeckIndex + 1];

				ExerTime+=Time.deltaTime;
				if (gauge >= Deck[DeckIndex+1]/2  && gauge <= (Deck[DeckIndex+1]/2) + 0.1f &&  ((Deck[DeckIndex]) != 0))
				{
					stateSnd.HalfPlay();
				}
				if (gauge >= Deck[DeckIndex+1] - 3.8f && gauge <= Deck[DeckIndex+1] - 3.7f && (Deck[DeckIndex]) == 0 )
				{
					Debug.Log(gauge);
					stateSnd.BeepPlay();
				}
				
				if (gauge + Mathf.Epsilon >= Deck[DeckIndex+1]) //운동시간 or 휴식시간이 끝났을때
				{
					gauge = 0;
					bar.SetActive(false);
					waitBar.SetActive(false);
					Solution.Callback = null;
					Destroy(_poseClassifierPrefab?.gameObject);
					_poseClassifierPrefab = null;
					// 마지막 운동은 쉬는시간이 없어야함
					stage = 1;
					if ((Deck[DeckIndex]) != 0) //0은 휴식임. 0이 아닌경우 운동진행한것.=>부위별로 경험치피드. all 같은경우 전신에 경험치 피드하기?
					{
						// 각 부위별 경험치량을 모두 더해준다. upper,core,lower순
						for (int i = 2; i <= 4; i++)
						{
							every_set_exp[i-2]+=int.Parse(ExerReader.ExerDic[Deck[DeckIndex].ToString()].Info[i]); //upper,core,lower순
						}
					}

					if (DeckIndex + 1 == Len - 1) // 마지막 휴식시간은 굳이 안해도된다.
					{
						if (IsDailyQuest) DailyQuestClear=true;
						stage = 3;
					}
					else
					{
						 DeckIndex += 2; // 두칸 건너뛰어야 다음 운동 애니메이션 or 쉬는 시간 인덱스로
						 JustOneRead=1;
					}
				}
			}
			else if (stage == 3) // 운동 끝 , 결과창 보이는 동시에 임계점 넘으면 blendershape 바꾸기,임계점은 각 부위별로 체크한다.
			{
				for (int i=0;i<3;i++)
				{
					int UpdatedExp=Mathf.Min(every_set_exp[i]+last_exp[i],100);
					ServerSender.ExpBlender.Add(UpdatedExp); // 서버로 보낼 데이터
				}

				Debug.Log("피드 경험치: "+every_set_exp[0]+" "+every_set_exp[1]+" "+every_set_exp[2]);
				
				// 운동 피드 경험치 sqlite에 피드하기
				DataDB.DBCommand(string.Format("UPDATE user_info SET upper={0},core={1},lower={2} WHERE user_id=\"{3}\"",ServerSender.ExpBlender[0],ServerSender.ExpBlender[1],ServerSender.ExpBlender[2],"user")); // 경험치 업데이트 후 블렌더 계수 sqlite에서 업뎃하고 이곳에 뿌려주어야 한다.
				//경험치 업데이트된 부분은 운동 날짜로그 DB에 저장.
				UpdateTime=(Int32)(DateTime.UtcNow-new DateTime(1970,1,1,0,0,0,0)).TotalMinutes;
				
				if (every_set_exp[0]>0) // 상체경험치 피드됐으면
				{
					DataDB.DBCommand(string.Format("UPDATE user_log SET upper_log={0},upper_loss=\"{1}\" WHERE user_id=\"user\"",UpdateTime,"false"));
					DataDB.LowerLoss="false";
				}

				if (every_set_exp[1]>0) // 코어경험치 피드됐으면
				{
					DataDB.DBCommand(string.Format("UPDATE user_log SET core_log={0},core_loss=\"{1}\" WHERE user_id=\"user\"",UpdateTime,"false"));
					DataDB.UpperLoss="false";
				}

				if (every_set_exp[2]>0) // 하체경험치 피드됐으면
				{
					DataDB.DBCommand(string.Format("UPDATE user_log SET lower_log={0},lower_loss=\"{1}\" WHERE user_id=\"user\"",UpdateTime,"false"));
					DataDB.CoreLoss="false";
				}

				if (IsDailyQuest && DailyQuestClear)
				{
					IsDailyQuest=false; //데일리퀘스트 false로 초기화. 더못하게 해야함.
					DailyQuestClear=false;
					// 데일리 퀘스트 완수 날짜로그, 이전 quest_num+1만큼 user_info에 업데이트
					DataDB.DailyUpdate(UpdateTime);
				}

				DataDB.DB_Read(string.Format("SELECT lower,upper,core,quest_num,quest_log FROM user_info WHERE user_id=\"{0}\"","user"),"user_info"); //업뎃된 유저 정보 가져오라 ㅋ
				
				// 현재 blendershape값에서 추가된 경험치를 빼라
				BlenderValue[0]=Mathf.Max(BlenderValue[0]-every_set_exp[0],0);
				BlenderValue[1]=Mathf.Max(BlenderValue[1]-every_set_exp[1],0);
				BlenderValue[2]=Mathf.Max(BlenderValue[2]-every_set_exp[2],0);

				DataDB.DBCommand(string.Format("UPDATE user_blender SET upper_blender={0},core_blender={1},lower_blender={2} WHERE user_id=\"{3}\"",BlenderValue[0],BlenderValue[1],BlenderValue[2],"user"));
				_blendShape.UpdateCharacterBlender();
				model_ani.SetInteger("pose", 100);
				backBtn.SetActive(false);
				bar.SetActive(false);
				waitBar.SetActive(false);

				ExerTime=Math.Truncate(ExerTime);
				
				//_chatText.text = "운동 끝\n"+$"운동 시간:{Math.Round(ExerTime,2)}초";
				_chatText.text = "운동 끝\n"+$"운동 시간:{Math.Truncate(ExerTime/60)}분 {ExerTime%60}초";
				ExerTime=0;

				exerciseReport.SetActive(true);

				// 만약 어느 한 부위라도 최대 경험치량에 도달했다면?
				if (DataDB.UserInfos[0].Upper>=100 && DataDB.UserInfos[0].Core>=100 && DataDB.UserInfos[0].Lower>=100)
					_chatText.text += "\n모든 부위 최대 능력치 도달!";
				else if (DataDB.UserInfos[0].Upper >= 100 || DataDB.UserInfos[0].Core >= 100 || DataDB.UserInfos[0].Lower >= 100)
					_chatText.text += string.Format("\n 각 부위 {0}이상 성장불가!", 200);
				
				stage = 4;
			}
			else if (stage == 4)
			{
			}
			else if (stage == 5)
			{
				gauge=0; // 게이지 초기화 필수. 중도에 그만두면 처음부터 다시해야하기 때문.
				moreExerciseBtn.SetActive(true);
				deckViewCanvas.SetActive(true);
				mainScrollView.vertical = true;
				model_ani.SetInteger("pose", 0);
				_chatText.text = "";
				exerciseReport.SetActive(false);
				swipeBar.SetActive(true); //다시 밀어넘기는 바 생성되고 처음인 stage0으로 되돌아감.
				isMoveContent = false;
				stage = 0;
				DeckIndex = 0; //덱 인덱스 초기화
				// 피드된 데이터가 last_exp가 된다.

				last_exp[0]=DataDB.UserInfos[0].Upper;
				last_exp[1]=DataDB.UserInfos[0].Core;
				last_exp[2]=DataDB.UserInfos[0].Lower;
				Debug.Log("상체"+last_exp[0]+" 코어"+last_exp[1]+" 하체"+last_exp[2]);
				ServerSender.ExpBlender.Clear();
				ServerSender.UserLog.Clear();
				
				for (int i=0;i<parts.Count;i++)
				{
					FindLack[parts[i]]=last_exp[i]; // 세트 운동 후에도 부위중 가장 부족한 곳이 어딘지 알기위해 필요한 로직.
				}
				
				Deck =new List<int>();
				for (int i=0;i<every_set_exp.Count;i++) every_set_exp[i]=0;
				card_name = ""; // 덱 이름 초기화
				//just_one_send=0; // 다음세트에도 요청을 보내야하니까 0으로 초기화
				JustOneRead=0;
				DataSend=true;
				TotalExpUI.SetActive(true);
			}

			if (isMoveContent)
			{
				RectTransform mainContentTransform = mainContent.transform as RectTransform;
				mainContentTransform.anchoredPosition = Vector2.Lerp(mainContentTransform.anchoredPosition, new Vector2(0, ContentYPos), Time.deltaTime * 5);
			}
		}

		IEnumerator cancleMoveContent()
		{
			yield return new WaitForSeconds(0.6f);
			isMoveContent = false;
			//mainScrollView.vertical = true;
		}

		void moveContent(float yPos)
		{
			//mainScrollView.vertical = false;
			isMoveContent = true;
			ContentYPos = yPos;
			StartCoroutine(cancleMoveContent());
		}

		public void moreSetBtn()
		{
			moveContent(1850);
		}

		public void backBtnClick()
		{
			rllyFinish.SetActive(true);
			darkBlur.SetActive(true);
			Time.timeScale = 0;
		}
		public void rllyFinishYes()
		{
			stage = 3;
			rllyFinish.SetActive(false);
			darkBlur.SetActive(false);
			Time.timeScale = 1;
		}

		public void rllyFinishNo()
		{
			rllyFinish.SetActive(false);
			darkBlur.SetActive(false);
			Time.timeScale = 1;
		}

		public void deckPreviewRightDrag() // 프리뷰 종료전에 덱의 총시간, 총 얻는 경험치량 정보 초기화 필수, 인덱스 정보도 초기화
		{
			card_name = DeckBtnManager.DeckName; 
			Deck = ExerSet_CSV_Reader.ExerSet[card_name];
			stage=1;
			
			DeckBtnManager.AllSetTime=0;
			for (int i=0;i<DeckBtnManager.SetPartExp.Count;i++)
			{
				DeckBtnManager.SetPartExp[i]=0;
			}
			DeckPreview.ExerNameStartIdx=0;
			DeckPreview.ExerTimeStartIdx=1;

			deckPreview.SetActive(false);
		}

		public void deckPreviewLefttDrag() // 프리뷰 종료전에 덱의 총시간, 총 얻는 경험치량 정보 초기화 필수, 인덱스 정보도 초기화
		{
			DeckBtnManager.AllSetTime=0; 
			for (int i=0;i<DeckBtnManager.SetPartExp.Count;i++)
			{
				DeckBtnManager.SetPartExp[i]=0;
			}
			DeckPreview.ExerNameStartIdx=0;
			DeckPreview.ExerTimeStartIdx=1;

			deckPreview.SetActive(false);
		}

		public void missionCard()
		{
			int SetNums=12; // 가지고있는 세트수. 일단 12인데 추후에 30개로 수정예정.
			stage = 1;
			card_name = ExerSet_CSV_Reader.SetKeys[DayValue%SetNums];
			Debug.Log(card_name);
			//now_exercise = 1;
			//swipe카드 실행시 card_name(운동 부위)키에서 덱 정보 문자열을 ExerciseManager의 Deck에 저장한다.
			Deck = ExerSet_CSV_Reader.ExerSet[card_name]; 
		}
	}
}
*/