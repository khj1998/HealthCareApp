using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using SOMA.StartTasks;
using SOMA.Managers;

namespace SOMA.Managers
{
	[SerializeField]
	public class UserInfos
	{
		public int Upper;
		public int Core;
		public int Lower;
		public int GraphStartIdx;
		public int GraphEndIdx;
		public List<int> GraphDatas;
		public int SelectedModel;

		public UserInfos(int upper=0,int core=0,int lower=0,int graphStartIdx=0,int graphEndIdx=-1,int firstExerCount=0,int firstChallengeCount=0,int selectedModel=-1)
    	{
        	Upper=upper;
		  	Core=core;
        	Lower=lower;
        	GraphStartIdx = graphStartIdx;
			GraphEndIdx = graphEndIdx;

			GraphDatas = new List<int>();
			SelectedModel = selectedModel;
		}
	}

	[SerializeField]
	public class UserBlender
	{
		public int UpperBlender;
		public int CoreBlender;
		public int LowerBlender;

		public UserBlender(int upperblender=-1,int coreblender=-1,int lowerblender=-1)
		{
			UpperBlender=upperblender;
			CoreBlender=coreblender;
			LowerBlender=lowerblender;
		}
	}

	[SerializeField]
	public class UserLog
	{
		public int UpperLog;
		public int CoreLog;
		public int LowerLog;
		public int LastExerLog;

		public UserLog(int upperlog=-1,int corelog=-1,int lowerlog=-1,int lastExerLog=-1)
		{
			UpperLog=upperlog;
			CoreLog=corelog;
			LowerLog=lowerlog;
			LastExerLog = lastExerLog;
		}
	}

	[SerializeField]
	public class ChallengeScores
	{
		public int SquatBestScore;
		public int PushUpBestScore;

		public ChallengeScores(int squatBestScore=-1,int pushUpBestScore=-1)
		{
			SquatBestScore = squatBestScore;
			PushUpBestScore = pushUpBestScore;
		}
	}

	[SerializeField]
	public class InAppProducts
    {
        public bool NewCharacter;
        public bool RemoveBanner;

        public InAppProducts(bool newCharacter=false,bool removeBanner=false)
        {
            this.NewCharacter = newCharacter;
            this.RemoveBanner = removeBanner;
        }
	}

	public class ReportDatas
	{
		public string Title;
		public double TotalExerciseTime;
		public int TotalGainExp;
		public int UpperGainExp;
		public int CoreGainExp;
		public int LowerGainExp;
	}

	public class ChallengeReportDatas
	{
		public int NowScore;
		public int ChallengeExerTime;
		public int NowChallengeExp;
	}

	public class DataManager : MonoBehaviourSingleton<DataManager>
	{
		private string _userinfospath
		{
			get {return "UserInfos"+".es3";}
		}

		private string _userblenderpath
		{
			get {return "UserBlender"+".es3";}
		}

		private string _userlogpath
		{
			get {return "UserLog"+".es3";}
		}

		private string _challengeScoresPath
		{
			get {return "ChallengeScores"+".es3";}
		}

		private string _inAppProductsPath
		{
			get {return "InAppProducts"+".es3";}
		}
		
		public UserInfos UserInfosTable;
		public UserBlender UserBlenderTable;
		public UserLog UserLogTable;
		public ChallengeScores ChallengeScoresTable;
		public InAppProducts InAppProductsTable;
		public ReportDatas ReportDataStorage;
		public ChallengeReportDatas ChallengeReportDataStorage;

		public bool UpperLoss;
		public bool CoreLoss;
		public bool LowerLoss;
		private int nowDate;
		private int dateDiff;
		private int updateTime;
		private bool exerciseDone;
		
		//public List<int> LastExp;
		//public List<int> BlenderValue;
		public List<int> EveryPartExp;

		public float SingleExerCount;
		public bool IsFirstLoadApp;
		public bool previewOn;
		public bool IsPermissionGranted;

		private int lossExpAmount;
		private int plusBlenderPoint;

		protected override void WhenAwake()
		{
			previewOn = false;
			exerciseDone = false;
			
			MakeDBFile();
			ReadUserInfos();
			ReadUserBlender();
			ReadUserLog();
			ReadChallengeScore();
			ReadInAppProucts();
			
			/*LastExp.Add(UserInfosTable.Upper);
			LastExp.Add(UserInfosTable.Core);
			LastExp.Add(UserInfosTable.Lower);

			BlenderValue.Add(UserBlenderTable.UpperBlender);
			BlenderValue.Add(UserBlenderTable.CoreBlender);
			BlenderValue.Add(UserBlenderTable.LowerBlender);*/

			for (int i = 0; i < 3; ++i)
			{
				EveryPartExp.Add(0);
			}
			
			if (!FirebaseManager.Instance.ChangeAccount)
			{
				ExerReader.Card_Read();
				ExerSet_CSV_Reader.ExerSet_Read();
			}

			ReportDataStorage = new ReportDatas();
			ChallengeReportDataStorage = new ChallengeReportDatas();
		}
		
		private void Start()
		{
			//Loss_Exp();
		}

		/*public void Loss_Exp() // 근손실 여부 판단하고 손실났으면 경험치 빼주고 블렌더계수는 그만큼 더해줌.
		{
			int now=(int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalDays;
			int dateDiff;

			if (UserLogTable.UpperLog != -1) // 아예 앱이 처음이거나 경험치가 0이면? 경험치 손실 로직 적용x
			{
				dateDiff = now - UserLogTable.UpperLog;

				if (dateDiff > 2 && LastExp[0] > 0)
				{
					UpperLoss=true;
					lossExpAmount = LastExp[0]/2;
					plusBlenderPoint = lossExpAmount/2;

					LastExp[0]=Mathf.Max(0,LastExp[0]-lossExpAmount); // 경험치가 0 미만으로 떨어지는 것 방지
					BlenderValue[0]=Mathf.Min(100,BlenderValue[0]+plusBlenderPoint); // 블렌더 계수가 100보다 커지는것 방지

					UserLogTable.UpperLog = now;
					
				}
			}

			if (UserLogTable.CoreLog != -1)
			{
				dateDiff = now - UserLogTable.CoreLog;
		
				if (dateDiff > 2 && LastExp[1] > 0)
				{
					CoreLoss=true;
					lossExpAmount = LastExp[1]/2;
					plusBlenderPoint = lossExpAmount/2;

					LastExp[1]=Mathf.Max(0,LastExp[1]-lossExpAmount); // 경험치가 0 미만으로 떨어지는 것 방지
					BlenderValue[1]=Mathf.Min(100,BlenderValue[1]+plusBlenderPoint); // 블렌더 계수가 100보다 커지는것 방지

					UserLogTable.CoreLog = now;
					
				}
			}

			if (UserLogTable.LowerLog!=-1)
			{
				dateDiff = now - UserLogTable.LowerLog;

				if (dateDiff > 2 && LastExp[2] > 0)
				{
					LowerLoss = true;
					lossExpAmount = LastExp[2]/2;
					plusBlenderPoint = lossExpAmount/2;

					LastExp[2]=Mathf.Max(0,LastExp[2]-lossExpAmount);
					BlenderValue[2]=Mathf.Min(100,BlenderValue[2]+plusBlenderPoint);

					UserLogTable.LowerLog = now;

				}
			}

			// 손실이 발생했을 경우에만 데이터 업데이트 && 백업을 진행해야한다.
			if (UpperLoss || CoreLoss || LowerLoss)
			{
				updateLossExp();
				updateLossUserBlender();
				updateLossUserLog();
			}
		}*/

		void updateLossExp()
		{
			/*UserInfosTable.Upper = LastExp[0];
			UserInfosTable.Core = LastExp[1];
			UserInfosTable.Lower = LastExp[2];*/
			
			ES3.Save("UserInfos" , (UserInfos)UserInfosTable , _userinfospath);

			#if UNITY_EDITOR
			Debug.Log("");
			#elif UNITY_ANDROID
			//SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateUserInfo(SOMA.Managers.FirebaseManager.Instance.IdToken,LastExp[0],LastExp[1],LastExp[2]);
			#endif
		}
		
		void updateLossUserBlender()
		{
			/*UserBlenderTable.UpperBlender = BlenderValue[0];
			UserBlenderTable.CoreBlender = BlenderValue[1];
			UserBlenderTable.LowerBlender = BlenderValue[2];*/

			ES3.Save("UserBlender" , (UserBlender)UserBlenderTable , _userblenderpath);

			#if UNITY_EDITOR
			Debug.Log("");
			#elif UNITY_ANDROID
			//SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateUserBlender(SOMA.Managers.FirebaseManager.Instance.IdToken,BlenderValue[0],BlenderValue[1],BlenderValue[2]);
			#endif
		}

		void updateLossUserLog()
		{
			ES3.Save("UserLog" , (UserLog)UserLogTable , _userlogpath);
		}

		void MakeDBFile()
		{
			if (!ES3.KeyExists("UserInfos",_userinfospath))
			{
				UserInfosTable = new UserInfos();
				ES3.Save("UserInfos",(UserInfos)UserInfosTable,_userinfospath);
				Debug.Log("UserInfos 최초 생성");
			}

			if (!ES3.KeyExists("UserBlender",_userblenderpath))
			{
				UserBlenderTable = new UserBlender(100,100,100);
				ES3.Save("UserBlender",(UserBlender)UserBlenderTable,_userblenderpath);
				Debug.Log("UserBlender 최초 생성");
			}

			if (!ES3.KeyExists("UserLog",_userlogpath))
			{
				UserLogTable = new UserLog(-1,-1,-1,-1);
				ES3.Save("UserLog",(UserLog)UserLogTable,_userlogpath);
				Debug.Log("UserLog 최초 생성");
			}

			if (!ES3.KeyExists("ChallengeScores",_challengeScoresPath))
			{
				ChallengeScoresTable = new ChallengeScores(0,0);
				ES3.Save("ChallengeScores",(ChallengeScores)ChallengeScoresTable,_challengeScoresPath);
				Debug.Log("ChallengeScores 최초 생성");
			}

			if (!ES3.KeyExists("InAppProducts",_inAppProductsPath))
			{
				InAppProductsTable = new InAppProducts(false,false);
				ES3.Save("InAppProducts",(InAppProducts)InAppProductsTable,_inAppProductsPath);
				IsFirstLoadApp = true;
				Debug.Log("InAppProducts 최초 생성");
			}
		}

		public void ReadUserInfos()
		{
			UserInfosTable = (UserInfos)ES3.Load("UserInfos",_userinfospath);
		}

		public void ReadUserBlender()
		{
			UserBlenderTable = (UserBlender)ES3.Load("UserBlender",_userblenderpath);
		}

		public void ReadUserLog()
		{
			UserLogTable = (UserLog)ES3.Load("UserLog",_userlogpath);
		}

		public void ReadChallengeScore()
		{
			ChallengeScoresTable = (ChallengeScores)ES3.Load("ChallengeScores",_challengeScoresPath);
		}

		public void ReadInAppProucts()
		{
			InAppProductsTable = (InAppProducts)ES3.Load("InAppProducts",_inAppProductsPath);
		}

		public void UpdateAfterExerUserExp(float achievenmentRate)
		{
			UserInfosTable.Upper = Mathf.Min((int)Math.Ceiling(EveryPartExp[0]*achievenmentRate)+UserInfosTable.Upper,200);
			UserInfosTable.Core = Mathf.Min((int)Math.Ceiling(EveryPartExp[1]*achievenmentRate)+UserInfosTable.Core,200);
			UserInfosTable.Lower = Mathf.Min((int)Math.Ceiling(EveryPartExp[2]*achievenmentRate)+UserInfosTable.Lower,200);

			WriteUserInfosFile();

			if (SOMA.firebaseDB.ConnectFirebaseDB.Instance.reference!=null)
			{
				// 업데이트된 경험치 정보 firebase 데이터베이스에 백업
				#if UNITY_EDITOR
				Debug.Log("");
				#elif UNITY_ANDROID
				SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateUserInfo(SOMA.Managers.FirebaseManager.Instance.IdToken,UserInfosTable.Upper,UserInfosTable.Core,UserInfosTable.Lower);
				#endif
			}
		}

		public void UpdateAfterExerUserBlender(float achievenmentRate)
		{
			UserBlenderTable.UpperBlender = Mathf.Max(UserBlenderTable.UpperBlender-(int)Math.Ceiling((EveryPartExp[0]*achievenmentRate)/2),0);
			UserBlenderTable.CoreBlender = Mathf.Max(UserBlenderTable.CoreBlender-(int)Math.Ceiling((EveryPartExp[1]*achievenmentRate)/2),0);
			UserBlenderTable.LowerBlender = Mathf.Max(UserBlenderTable.LowerBlender-(int)Math.Ceiling((EveryPartExp[2]*achievenmentRate)/2),0);

			WriteUserBlenderFile();

			Debug.Log(SOMA.firebaseDB.ConnectFirebaseDB.Instance.reference);
			if (SOMA.firebaseDB.ConnectFirebaseDB.Instance.reference!=null)
			{
				#if UNITY_EDITOR
				Debug.Log("");
				#elif UNITY_ANDROID
				SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateUserBlender(SOMA.Managers.FirebaseManager.Instance.IdToken,UserBlenderTable.UpperBlender,UserBlenderTable.CoreBlender,UserBlenderTable.LowerBlender);
				#endif
			}
		}

		public void UpdateAfterExerUserLog()
		{
			updateTime = (Int32)(DateTime.UtcNow - new DateTime(1970,1,1,0,0,0,0)).TotalDays;

			if (EveryPartExp[0]>0)
			{
				exerciseDone = true;
				UserLogTable.UpperLog = updateTime;
				UpperLoss = false;
			}

			if (EveryPartExp[1]>0)
			{
				exerciseDone = true;
				UserLogTable.CoreLog = updateTime;
				CoreLoss = false;
			}

			if (EveryPartExp[2]>0)
			{
				exerciseDone = true;
				UserLogTable.LowerLog = updateTime;
				LowerLoss = false;
			}

			if (exerciseDone)
			{
				UserLogTable.LastExerLog = updateTime;
				exerciseDone = false;
			}
		}

		public void UpdateAfterChallenge(int addScore)
		{
			UserInfosTable.Upper = Mathf.Min(100,addScore/2+UserInfosTable.Upper);
			UserInfosTable.Core = Mathf.Min(100,addScore/2+UserInfosTable.Core);
			UserInfosTable.Lower = Mathf.Min(100,addScore/2+UserInfosTable.Lower);

			UserBlenderTable.UpperBlender = Mathf.Max(UserBlenderTable.UpperBlender-addScore/2,0);
			UserBlenderTable.CoreBlender = Mathf.Max(UserBlenderTable.CoreBlender-addScore/2,0);
			UserBlenderTable.LowerBlender = Mathf.Max(UserBlenderTable.LowerBlender-addScore/2,0);

			WriteUserInfosFile();
			WriteUserBlenderFile();

			#if UNITY_EDITOR
			Debug.Log("");
			#elif UNITY_ANDROID
			SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateUserInfo(SOMA.Managers.FirebaseManager.Instance.IdToken,UserInfosTable.Upper,UserInfosTable.Core,UserInfosTable.Lower);
			SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateUserBlender(SOMA.Managers.FirebaseManager.Instance.IdToken,UserBlenderTable.UpperBlender,UserBlenderTable.CoreBlender,UserBlenderTable.LowerBlender);
			#endif
		}

		public void UpdateChallengeScore(int score,string exercise)
		{
			if (exercise == "squat")
			{
				ChallengeScoresTable.SquatBestScore = score;
			}
			else if (exercise == "pushup")
			{
				ChallengeScoresTable.PushUpBestScore = score;
			}

			WriteChallengeScoresFile();
		}

		public void UpdateModelNumber(int modelNumber) // 쿼리문도 날려야함.
		{
			UserInfosTable.SelectedModel = modelNumber;
			WriteUserInfosFile();
		}

		public void UpdateDataAfterExer(float achievenmentRate)
		{
			UpdateAfterExerUserExp(achievenmentRate);
			UpdateAfterExerUserBlender(achievenmentRate);
			UpdateAfterExerUserLog();
		}

		public void WriteUserInfosFile()
		{
			ES3.Save("UserInfos",(UserInfos)UserInfosTable , _userinfospath);
		}

		public void WriteUserBlenderFile()
		{
			ES3.Save("UserBlender" , (UserBlender)UserBlenderTable , _userblenderpath);
		}

		public void WriteChallengeScoresFile()
		{
			ES3.Save("ChallengeScores",(ChallengeScores)ChallengeScoresTable,_challengeScoresPath);
		}

		public void WriteUserLogFile()
		{
			ES3.Save("UserLog" , (UserLog)UserLogTable , _userlogpath);
		}

		public void WriteUserProductFile()
		{
			ES3.Save("InAppProducts",(InAppProducts)InAppProductsTable,_inAppProductsPath);
		}

		public void InitDataInfos()
		{
			for (int i =0; i<3; i++)
			{
				EveryPartExp[i]=0;
			}
		}

		public void  GetGraphData(float achievenmentRate)
		{
			int totalExp=0;
			int nowGraphIdx = UserInfosTable.GraphEndIdx;

			for (int i=0;i<3;i++)
			{
				totalExp += (int)Math.Ceiling((DataManager.Instance.EveryPartExp[i])*achievenmentRate);
			}


			int nowLog = (Int32)(DateTime.UtcNow - new DateTime(1970,1,1,0,0,0,0)).TotalDays;
			int logDiff = nowLog - UserLogTable.LastExerLog;

			if (nowGraphIdx==-1)
			{
				UserInfosTable.GraphEndIdx+=1;
				UserInfosTable.GraphDatas.Add(totalExp);
				return;
			}

			if(logDiff==0)
			{
				Debug.Log("같은 날짜에 운동함");
				UserInfosTable.GraphDatas[nowGraphIdx]+=totalExp;
				return;
			}

			for (int i=nowGraphIdx+1;i<nowGraphIdx+logDiff;i++)
			{
				UserInfosTable.GraphDatas.Add(0);
			}

			UserInfosTable.GraphDatas.Add(totalExp);
			UserInfosTable.GraphEndIdx +=logDiff;

			if (UserInfosTable.GraphEndIdx - UserInfosTable.GraphStartIdx>6) // 그래프 밀어야함. StartIdx 증가
			{
				UserInfosTable.GraphStartIdx += (UserInfosTable.GraphEndIdx - UserInfosTable.GraphStartIdx-6);
			}
		}
		
		protected override void WhenDestroy() {}
	}
}
