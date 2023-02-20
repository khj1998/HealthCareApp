using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using System;
using SOMA.Managers;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SOMA.firebaseDB
{
    public class UserInfo
    {
        public int Upper;
		public int Core;
		public int Lower;
        public int SelectedModel;
 
        public UserInfo(int upper , int core, int lower,int selectedModel)
        {
            this.Upper = upper;
            this.Core = core;
            this.Lower = lower;
            this.SelectedModel = selectedModel;
        }

    }

    public class UserBlender
	{
		public int UpperBlender;
		public int CoreBlender;
		public int LowerBlender;

		public UserBlender(int upperblender,int coreblender,int lowerblender)
		{
			UpperBlender=upperblender;
			CoreBlender=coreblender;
			LowerBlender=lowerblender;
		}
	} 

    public class UserLog
	{
		public int UpperLog;
		public int CoreLog;
		public int LowerLog;
		public int LastExerLog;

		public UserLog(int upperlog,int corelog,int lowerlog,int lastExerLog)
		{
			UpperLog=upperlog;
			CoreLog=corelog;
			LowerLog=lowerlog;
			LastExerLog = lastExerLog;
		}
	}

	public class ChallengeScores
	{
		public int SquatBestScore;
		public int PushUpBestScore;

		public ChallengeScores(int squatBestScore,int pushUpBestScore)
		{
			SquatBestScore = squatBestScore;
			PushUpBestScore = pushUpBestScore;
		}
	}

    public class InAppProducts
    {
        public bool NewCharacter;
        public bool RemoveBanner;

        public InAppProducts(bool newCharacter,bool removeBanner)
        {
            this.NewCharacter = newCharacter;
            this.RemoveBanner = removeBanner;
        }

        public void buyCharacter(bool newCharacter)
        {
            this.NewCharacter = newCharacter;
        }

        public void buyRemoveBanner(bool removeBanner)
        {
            this.RemoveBanner = removeBanner;
        }
    }

    public class ConnectFirebaseDB : MonoBehaviourSingleton<ConnectFirebaseDB>
{
    public DatabaseReference reference;
    private List<string> userInfoBackUpList;
    private List<string> userBlenderBackUpList;
    public List<string> userChallengeScoresBackUpList;
    public List<string> inAppProductsBackupList;
    public ObscuredString userId;

    protected override void WhenAwake() {}
       
    private IEnumerator Start() 
    {
        yield return new WaitUntil(() => FirebaseManager.Instance.LoginFinish);
        Debug.Log("ConnectFirebaseDB 실행");

        reference = FirebaseDatabase.DefaultInstance.RootReference;
        userId = SOMA.Managers.FirebaseManager.Instance.IdToken;
        initBackUpList();
        
        // 처음 앱을 켜서 파일을 생성했다면? DB에 데이터를 새로 생성하거나 백업을 해줘야하는 상황.
		if (DataManager.Instance.IsFirstLoadApp && reference!=null)
		{
            #if UNITY_EDITOR
            Debug.Log("");
            #elif UNITY_ANDROID
			CreateNewDatas(userId);
            #endif
		}
        else
        {
            #if UNITY_EDITOR
            Debug.Log("");
            #elif UNITY_ANDROID
            BackUpUserInAppPurchase(userId);
            #endif
        }

        FirebaseManager.Instance.LoginFinish = false;
    }

    public void initBackUpList()
    {
        userInfoBackUpList = new List<string>();
        userBlenderBackUpList = new List<string>();
        userChallengeScoresBackUpList = new List<string>();
        inAppProductsBackupList = new List<string>();
    }
    
    private void BackUp()
    {
        if (reference!=null)
        {
            Debug.Log("백업을 진행합니다.");
            BackUpUserInfoBackUp(userId);
            BackUpUserBlender(userId);
            BackUpUserChallengeScore(userId);
            BackUpUserInAppPurchase(userId);
        }
    }

    public void CreateNewDatas(string userID)
    {
        // this method read data only once.
        reference.Child(userID).GetValueAsync().ContinueWith(task => 
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
    
                if (!snapshot.HasChildren)
                {
                    Debug.Log("해당 아이디가 없으므로 데이터를 생성합니다.");
                    CreateNewDatarWithJson(userID);
                }
                else
                {
                    Debug.Log("해당 유저는 백업할 데이터가 존재합니다.");
                    BackUp();
                }
            }
            else
            {
                Debug.Log("task failed");
            }
        });
    }

    // 로그아웃후 다른아이디 재접속시 실행되는 함수
    public void CheckIdExist(string userID)
    {
        reference.Child(userID).GetValueAsync().ContinueWith(task =>
        {   
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (!snapshot.HasChildren)
                {
                    Debug.Log("해당 유저의 데이터를 새로 생성합니다");
                    CreateNewDatarWithJson(userID);
                    ClearES3Datas();
                }
                else
                {
                    // 해당 아이디는 이미 데이터가 있음. 데이터 백업을 진행한다.
                    Debug.Log("해당 유저의 데이터 백업을 진행합니다 " + inAppProductsBackupList.Count);

                    BackUpUserInfoBackUp(userID);
                    BackUpUserBlender(userID);
                    BackUpUserChallengeScore(userID);
                    BackUpUserInAppPurchase(userID);
                }
            }
        });
    }

    private void ClearES3Datas()
    {
        DataManager.Instance.UserInfosTable.Upper = 0;
        DataManager.Instance.UserInfosTable.Core = 0;
        DataManager.Instance.UserInfosTable.Lower = 0;
        
        DataManager.Instance.UserInfosTable.GraphStartIdx = 0;
        DataManager.Instance.UserInfosTable.GraphEndIdx = -1;
        DataManager.Instance.UserInfosTable.SelectedModel = -1;
        DataManager.Instance.UserInfosTable.GraphDatas.Clear();
        DataManager.Instance.WriteUserInfosFile();

        DataManager.Instance.UserBlenderTable.UpperBlender = 100;
        DataManager.Instance.UserBlenderTable.CoreBlender = 100;
        DataManager.Instance.UserBlenderTable.LowerBlender = 100;
        DataManager.Instance.WriteUserBlenderFile();

        DataManager.Instance.UserLogTable.UpperLog = -1;
        DataManager.Instance.UserLogTable.CoreLog = -1;
        DataManager.Instance.UserLogTable.LowerLog = -1;
        DataManager.Instance.UserLogTable.LastExerLog = -1;
        DataManager.Instance.WriteUserLogFile();

        DataManager.Instance.ChallengeScoresTable.SquatBestScore = 0;
        DataManager.Instance.ChallengeScoresTable.PushUpBestScore = 0;
        DataManager.Instance.WriteChallengeScoresFile();

        DataManager.Instance.InAppProductsTable.NewCharacter = false;
        DataManager.Instance.InAppProductsTable.RemoveBanner = false;
        DataManager.Instance.WriteUserProductFile();
    }

    private void BackUpUserInfoBackUp(string userId)
    {
        reference.Child(userId).Child("UserInfo").GetValueAsync().ContinueWith(task => {

            if(task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;

                userInfoBackUpList.Add(snapshot.Child("Upper").Value.ToString());
                userInfoBackUpList.Add(snapshot.Child("Core").Value.ToString());
                userInfoBackUpList.Add(snapshot.Child("Lower").Value.ToString());
                userInfoBackUpList.Add(snapshot.Child("SelectedModel").Value.ToString());

                DataManager.Instance.UserInfosTable.Upper = Convert.ToInt32(userInfoBackUpList[0]);
                DataManager.Instance.UserInfosTable.Core = Convert.ToInt32(userInfoBackUpList[1]);
                DataManager.Instance.UserInfosTable.Lower = Convert.ToInt32(userInfoBackUpList[2]);

                DataManager.Instance.UserInfosTable.SelectedModel = Convert.ToInt32(userInfoBackUpList[3]);
                
                DataManager.Instance.WriteUserInfosFile();
                Debug.Log("UserInfo 백업");
            }
        });
    }

    private void BackUpUserBlender(string userId)
    {
        reference.Child(userId).Child("UserBlender").GetValueAsync().ContinueWith(task => {

            if(task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;

                userBlenderBackUpList.Add(snapshot.Child("UpperBlender").Value.ToString());
                userBlenderBackUpList.Add(snapshot.Child("CoreBlender").Value.ToString());
                userBlenderBackUpList.Add(snapshot.Child("LowerBlender").Value.ToString());

                DataManager.Instance.UserBlenderTable.UpperBlender = Convert.ToInt32(userBlenderBackUpList[0]);
                DataManager.Instance.UserBlenderTable.CoreBlender = Convert.ToInt32(userBlenderBackUpList[1]);
                DataManager.Instance.UserBlenderTable.LowerBlender = Convert.ToInt32(userBlenderBackUpList[2]);

                DataManager.Instance.WriteUserBlenderFile();
                Debug.Log("UserBlender 백업");
            }
        });
    }

    private void BackUpUserChallengeScore(string userId)
    {
        reference.Child(userId).Child("ChallengeScores").GetValueAsync().ContinueWith(task => {

            if(task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;

                userChallengeScoresBackUpList.Add(snapshot.Child("PushUpBestScore").Value.ToString());
                userChallengeScoresBackUpList.Add(snapshot.Child("SquatBestScore").Value.ToString());

                DataManager.Instance.ChallengeScoresTable.PushUpBestScore = Convert.ToInt32(userChallengeScoresBackUpList[0]);
                DataManager.Instance.ChallengeScoresTable.SquatBestScore = Convert.ToInt32(userChallengeScoresBackUpList[1]);

                DataManager.Instance.WriteChallengeScoresFile();
                Debug.Log("챌린지 백업");
            }
        });
    }

    private void BackUpUserInAppPurchase(string userId)
    {
        reference.Child(userId).Child("InAppPurchase").GetValueAsync().ContinueWith(task => {
            
            if(task.IsCompletedSuccessfully)
            {
                DataSnapshot snapshot = task.Result;

                inAppProductsBackupList.Add(snapshot.Child("NewCharacter").Value.ToString());
                inAppProductsBackupList.Add(snapshot.Child("RemoveBanner").Value.ToString());
                Debug.Log("RemoveBanner 구매내역: "+inAppProductsBackupList[1]);

                DataManager.Instance.InAppProductsTable.NewCharacter = Convert.ToBoolean(inAppProductsBackupList[0]);
                DataManager.Instance.InAppProductsTable.RemoveBanner = Convert.ToBoolean(inAppProductsBackupList[1]);

                DataManager.Instance.WriteUserProductFile();
                Debug.Log("구매내역 백업");
            }
        });
    }

    private void CreateNewDatarWithJson(string userID)
    {
        UserInfo userInfo = new UserInfo(0,0,0,-1);
        string json = JsonUtility.ToJson(userInfo);
        reference.Child(userID).Child("UserInfo").SetRawJsonValueAsync(json);
        
        UserBlender userBlender = new UserBlender(100,100,100);
        string json2 = JsonUtility.ToJson(userBlender);
        reference.Child(userID).Child("UserBlender").SetRawJsonValueAsync(json2);

        ChallengeScores challengeScores = new ChallengeScores(0,0);
        string json3 = JsonUtility.ToJson(challengeScores);
        reference.Child(userID).Child("ChallengeScores").SetRawJsonValueAsync(json3);

        InAppProducts inAppProducts = new InAppProducts(false,false);
        string json4 = JsonUtility.ToJson(inAppProducts);
        reference.Child(userID).Child("InAppPurchase").SetRawJsonValueAsync(json4);

        Debug.Log("유저의 새로운 데이터를 생성합니다.");
    }

    public void UpdateUserInfo(string userId,int upper,int core,int lower)
    {
        // Create new entry at /user-scores/$userid/$scoreid and at
        // /leaderboard/$scoreid simultaneously
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["Upper"]=upper;
        childUpdates["Core"]=core;
        childUpdates["Lower"]=lower;

        reference.Child(userId).Child("UserInfo").UpdateChildrenAsync(childUpdates).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("성공여부"+task.IsCompletedSuccessfully);
            }
            else
            {
                Debug.Log("데이터 업데이트 실패");
            }
            return;
        });
    }

    public void UpdateModelNumber(string userId,int modelNumber)
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["SelectedModel"] = modelNumber;

        reference.Child(userId).Child("UserInfo").UpdateChildrenAsync(childUpdates).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("성공여부"+task.IsCompletedSuccessfully);
            }
            else
            {
                Debug.Log("데이터 업데이트 실패");
            }
            return;
        });
    }

    public void UpdateUserBlender(string userId,int upperblender,int coreblender,int lowerblender)
    {
        Dictionary<string,object> childUpdates = new Dictionary<string, object>();
        childUpdates["UpperBlender"]=upperblender;
        childUpdates["CoreBlender"]=coreblender;
        childUpdates["LowerBlender"]=lowerblender;

        reference.Child(userId).Child("UserBlender").UpdateChildrenAsync(childUpdates).ContinueWith(task => 
        {
            if (task.IsCompleted)
            {
                Debug.Log("성공여부"+task.IsCompletedSuccessfully);
            }
            else
            {
                Debug.Log("데이터 업데이트 실패");
            }
            return;
        });
    }

    public void UpdateSquatScore(string userId, int squatbestscore) 
    {
    // Create new entry at /user-scores/$userid/$scoreid and at
    // /leaderboard/$scoreid simultaneously
    Dictionary<string, object> childUpdates = new Dictionary<string, object>();
    childUpdates["SquatBestScore"]=squatbestscore;

    reference.Child(userId).Child("ChallengeScores").UpdateChildrenAsync(childUpdates).ContinueWith(task =>
    {
        if (task.IsCompleted)
        {
            Debug.Log("성공여부"+task.IsCompletedSuccessfully);
        }
        else
        {
            Debug.Log("데이터 업데이트 실패");
        }
        return;
    });
    }

    public void UpdatePushUpScore(string userId, int pushupbestscore) 
    {
    // Create new entry at /user-scores/$userid/$scoreid and at
    // /leaderboard/$scoreid simultaneously
    Dictionary<string, object> childUpdates = new Dictionary<string, object>();
    childUpdates["PushUpBestScore"]=pushupbestscore;

    reference.Child(userId).Child("ChallengeScores").UpdateChildrenAsync(childUpdates).ContinueWith(task =>
    {
        if (task.IsCompleted)
        {
            Debug.Log("데이터 업데이트 성공");
        }
        else
        {
            Debug.Log("데이터 업데이트 실패");
        }
        return;
    });
    }

    public void UpdateRemoveAdsInAppPurchase(string userId,bool isBuying)
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["RemoveBanner"]=isBuying;

        reference.Child(userId).Child("InAppPurchase").UpdateChildrenAsync(childUpdates).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("데이터 업데이트 성공");
            }
            else
            {
                Debug.Log("데이터 업데이트 실패");
            }
            return;
        });
    }

    public void UpdateCharacterInAppPurchase(string userId,bool isBuying)
    {
        Dictionary<string, object> childUpdates = new Dictionary<string, object>();
        childUpdates["NewCharacter"]=isBuying;

    reference.Child(userId).Child("InAppPurchase").UpdateChildrenAsync(childUpdates).ContinueWith(task =>
    {
        if (task.IsCompleted)
        {
            Debug.Log("데이터 업데이트 성공");
        }
        else
        {
            Debug.Log("데이터 업데이트 실패");
        }
        return;
    });
    }

    protected override void WhenDestroy(){}
}
}
