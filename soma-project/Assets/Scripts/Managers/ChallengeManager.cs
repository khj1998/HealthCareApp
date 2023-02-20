using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SOMA;
using SOMA.UI;
using TMPro;
using SOMA.MediaPipe;
using SOMA.MediaPipe.Classification;
using SOMA.Managers;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SOMA
{
    public class ChallengeManager : MonoBehaviour
    {
        [SerializeField]
        private UIOnOffManager uiOnOffManager;

        [SerializeField]
        private GameObject pointEffect;

        [SerializeField]
        private PoseSolution poseSolution;

        [SerializeField]
        private Image _checkMark;

        [SerializeField]
        private PoseClassifierProcessor squatClassifierProcessor;

        [SerializeField]
        private PoseClassifierProcessor pushupClassifierProcessor;

        [SerializeField]
        private Bar timerBar;
        [SerializeField]
        private GameObject _challengeReport;

        [SerializeField]
		private GameObject modelGenerator;

        public TextMeshProUGUI timerTmp;
        public TextMeshProUGUI subTimerTmp;
        public TextMeshProUGUI timeOutTmp;
        public TextMeshProUGUI scoreTmp;
        private float challengeTime;
        private ObscuredFloat score;
        private float previousScore;
        private ObscuredInt challengeScore;
        private float inCameraTime;
        private bool isInCamera;
        private PoseClassifierProcessor poseClassifierProcessor;

        private float challengeMaxTime = 60.00f;
        private float aniSpeed = 0.5f;
        private float totalTimeBeforeGetScore = 0;
        public Animator ChallengeAnim;

        private bool justOneSend;
        private string userId;

        static public float lastGetPoint = 0;
        public enum State
        {
            guide,
            inCamera,
            ready,
            inWorkout,
            report,
            waitState // 특정 state에서 코드를 한번만 실행하기 위해 사용
        };

        // QuitChallenge 스크립트에서 참조함
        public static State state;

        private void SetClassifier()
        {
            squatClassifierProcessor.gameObject.SetActive(false);
            pushupClassifierProcessor.gameObject.SetActive(false);

            if (ChallengeToggle.IsSquatChallenge)
            {
                poseClassifierProcessor = squatClassifierProcessor;
                squatClassifierProcessor.gameObject.SetActive(true);
            }
            else if (ChallengeToggle.IsPushUpChalllenge)
            {
                poseClassifierProcessor = pushupClassifierProcessor;
                pushupClassifierProcessor.gameObject.SetActive(true);
            }
        }

        void Awake() 
        {
            // 일단 챌린지 모드에서 배너광고 안띄워봄
            //SOMA.Ads.BannerAds.Instance.DestroyBanner();
            userId = SOMA.Managers.FirebaseManager.Instance.IdToken;
            SetClassifier();
            state = State.guide;
            
            justOneSend = true;
            challengeTime = 0f;  // 파일에서 읽어 가져오고 업데이트 하는 방식으로 바꿀거임 ㅇㅇ. UserInfosTable에 추가예정
            challengeScore = 0;  // 파일에서 읽어 가져오고 업데이트 하는 방식으로 바꿀거임 ㅇㅇ. UserInfosTable에 추가예정
            timerBar.maxGauge = challengeMaxTime;
            timerTmp.text = challengeTime.ToString();
            scoreTmp.text =  challengeScore.ToString();

            poseSolution.StopScreen();
            poseSolution.StopAnnotation();
        }
        void Start()
        {
            ChallengeAnim = modelGenerator.transform.GetChild(0).GetComponent<Animator>();
            ChallengeAnim.SetFloat("aniSpeed", aniSpeed);
            //modelGenerator에서 생성한 모델에 접근
        }
        void Update()
        {
            timerTmp.text = $"{challengeMaxTime - challengeTime:F2}";
            scoreTmp.text =  $"{challengeScore}";
     
            scoreTmp.text =  $"{challengeScore}";

            if (state == State.guide)
            {
                //가이드라인 표시
                //가이드라인 마지막 "운동시작" 버튼을 누르면 state 를 incamera로 변경
            }
            else if (state == State.inCamera)
            {
                CheckInCamera();
            }
            else if (state == State.ready)
            {
                // TODO: checkmark 끄기
                StartCoroutine(show321go());
                state = State.waitState;

                if (SOMA.UI.ChallengeToggle.IsSquatChallenge)
                {
                    ChallengeAnim.SetInteger("pose", 15);
                }
                else
                {
                    ChallengeAnim.SetInteger("pose", 12);
                }
            }
            else if (state == State.inWorkout)
            {
                if (Input.GetKeyDown(KeyCode.Space) == true)
                {
                    getPoint(30);
                }
                //Timer.text = $"{3.00 - challengeTime:F0}";
                if (challengeTime < challengeMaxTime)
                {
                    challengeTime += Time.deltaTime;
                    timerTmp.text = string.Format("{0:D2}", (int)(challengeMaxTime - challengeTime));
                    subTimerTmp.text = string.Format("{0:D2}", (int)(((challengeMaxTime - challengeTime) % 1) * 100));
                }
                else
                {
                    timerTmp.text = "";
                    subTimerTmp.text = "";
                    timeOutTmp.text = " Timeout!";
                }
                timerBar.gauge = challengeMaxTime - challengeTime;

                if (challengeTime >= challengeMaxTime)
                {
                    challengeTime = challengeMaxTime;
                    StartCoroutine(SetDanceTimeAndReport());

                }

                if (score != previousScore)
                {
                    getPoint((int)(score - previousScore));
                    previousScore = score;
                }
            }
            else if (state == State.report)
            {
                poseSolution.StopAnnotation();

                #if !UNITY_EDITOR
                Debug.Log("챌린지 모드 아날리틱스");
                FirebaseManager.Instance.LogEvent("challenge_finished",new Firebase.Analytics.Parameter[]{
                    new Firebase.Analytics.Parameter("challenge_score", challengeScore),
                    new Firebase.Analytics.Parameter("challenge_type", ChallengeToggle.IsSquatChallenge ? "squat" : "push_up")
                });
                #endif
                
                if (justOneSend)
                {
                    DataManager.Instance.UpdateAfterChallenge(challengeScore/30);
                    justOneSend = false;
                    
                    if (checkBestScore() && SOMA.firebaseDB.ConnectFirebaseDB.Instance.reference!=null)
                    {
                        if (SOMA.UI.ChallengeToggle.IsSquatChallenge)
                        {
                            SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateSquatScore(userId,challengeScore);
                        }
                        else
                        {
                            SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdatePushUpScore(userId,challengeScore);
                        }
                    }
                }

                FillReportStorage();
                uiOnOffManager.TurnOnReportCanvas();
            }


            /*
            if (Input.GetKeyUp(KeyCode.Space) == true)
            {
                getPoint();
            }
            */
        }

        void CheckInCamera()
        {
            //카메라 안에 유저가 있어야함
            //카메라 안에 유저가 잡히면 state = State.ready
            if (isInCamera)
            {
                inCameraTime += Time.deltaTime;
            }
            else
            {
                inCameraTime = 0.0f;
            }

            _checkMark.fillAmount = Mathf.Clamp((inCameraTime - 0.5f) / 2.5f, 0.0f, 1.0f);
            //if (inCameraTime >= 3.0f)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                inCameraTime = 4;
            }
            if (inCameraTime >= 3.0f)
            {
                poseSolution.IsInsideCameraCallback = null;
                poseSolution.StopScreen();
                uiOnOffManager.screenBackGround.SetActive(false);
                uiOnOffManager.guideLineImage.SetActive(false);
                state = State.ready;
            }
        }

        public void FillReportStorage()
		{
            int tempChallengeTime = (Int32)Math.Round(challengeTime);
			DataManager.Instance.ChallengeReportDataStorage.NowScore = challengeScore;
			DataManager.Instance.ChallengeReportDataStorage.ChallengeExerTime = tempChallengeTime;
			DataManager.Instance.ChallengeReportDataStorage.NowChallengeExp = challengeScore/10;
		}


        bool checkBestScore()
        {
            bool result = false;
            int lastScore;

            if (SOMA.UI.ChallengeToggle.IsSquatChallenge)
            {
                lastScore = DataManager.Instance.ChallengeScoresTable.SquatBestScore;

                if (lastScore < challengeScore)
                {
                    result = true;
                    DataManager.Instance.ChallengeScoresTable.SquatBestScore = challengeScore;
                    DataManager.Instance.UpdateChallengeScore(challengeScore,"squat");
                }
            }

            else
            {
                lastScore = DataManager.Instance.ChallengeScoresTable.PushUpBestScore;

                if (lastScore < challengeScore)
                {
                    result = true;
                    DataManager.Instance.ChallengeScoresTable.PushUpBestScore = challengeScore;
                    DataManager.Instance.UpdateChallengeScore(challengeScore,"pushup");
                }
            }

            return result;
        }

        IEnumerator ReportOpen()
        {
            yield return new WaitForSeconds(1);
            //openReport;
        }
        IEnumerator SetDanceTimeAndReport()
        {
            ChallengeAnim.SetInteger("pose", 0);
            ChallengeAnim.SetInteger("pose", 100);
            yield return new WaitForSeconds(3.26f);
            ChallengeAnim.SetInteger("pose", 0);
            state = State.report;
            //openReport;
        }

        IEnumerator show321go()
        {
            yield return new WaitForSeconds(0.9f);
            _checkMark.fillAmount = 0.0f;
            uiOnOffManager.active321Go();
            yield return new WaitForSeconds(3.0f);
            uiOnOffManager.setChallengeTimerBar();
            state = State.inWorkout;
            score = previousScore = 0.0f;
            poseSolution.Callback = (landmarks) => score = poseClassifierProcessor.Classify(landmarks);

        }

        public void guideEndBtnClick()
        {
            inCameraTime = 0.0f;
            isInCamera = false;
            poseSolution.IsInsideCameraCallback = (isInCamera) => this.isInCamera = isInCamera;
            poseSolution.StartScreen();
            poseSolution.StartAnnotation();
            state = State.inCamera;
            //StartCoroutine(show321go());    

            if (uiOnOffManager is not null)
            {
                uiOnOffManager.offGuideCanvas();
            }
        }

        public void getPoint(int point)
        {
            challengeScore += point;
            lastGetPoint = point;
            aniSpeed = (-2.6f * (challengeTime - totalTimeBeforeGetScore) + 12.6f) * 0.2f;
            if (aniSpeed < 0.2f)
            {
                aniSpeed = 0.2f;
            }
            else if (aniSpeed > 3f)
            {
                aniSpeed = 3f;
            }
            ChallengeAnim.SetFloat("aniSpeed", aniSpeed);
            totalTimeBeforeGetScore = challengeTime;
            //Instantiate(pointEffect, new Vector2(rx, ry), Quaternion.identity);
            Instantiate(pointEffect,GameObject.Find("Canvas").transform);
        }
    }
}