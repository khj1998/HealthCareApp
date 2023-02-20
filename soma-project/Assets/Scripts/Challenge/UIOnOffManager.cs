using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SOMA;
using SOMA.UI;

namespace SOMA.UI
{
    public class UIOnOffManager : MonoBehaviour
    {
        [SerializeField]
        //챌린지
        private Canvas GuideCanvas;
        [SerializeField]
        private GameObject mainCanvas;
        [SerializeField]
        private GameObject ReportCanvas;
        [SerializeField]
        private GameObject go321;
        [SerializeField]
        private GameObject rllyBack;

        [SerializeField]
        private GameObject challengeTimerBar;
        [SerializeField]
        public GameObject screenBackGround;
        [SerializeField]
        public GameObject guideLineImage;



        // 루틴 운동
        [SerializeField]
        private GameObject nextBtn;
        [SerializeField]
        private GameObject nextBtnOff;

        /*[SerializeField]
        private GameObject restBar;
        [SerializeField]
        private Button skipRestBtn;*/

        [SerializeField]
        private GameObject restUIObjects;
        [SerializeField]
        private GameObject inWorkOutUIObjects;



        public void offGuideCanvas()
        {
            GuideCanvas.enabled = false;
        }
        public void setRest()
        {
            inWorkOutUIObjects.SetActive(false);
            restUIObjects.SetActive(true);
        }
        public void setWorkout()
        {
            nextBtnOff.SetActive(true);
            nextBtn.SetActive(false);
            restUIObjects.SetActive(false);
            inWorkOutUIObjects.SetActive(true);
        }
        public void setCanNextExercise()
        {
            nextBtn.SetActive(true);
            nextBtnOff.SetActive(false);
        }
        public void TurnOnReportCanvas()
        {
            ReportCanvas.SetActive(true);
        }
        public void active321Go()
        {
            go321.SetActive(true);
        }
        public void onClickBackBtn()
        {
            rllyBack.SetActive(true);
        }
        public void onClickCancelBack()
        {
            rllyBack.SetActive(false);
        }
        public void setChallengeTimerBar()
        {
            challengeTimerBar.SetActive(true);
            //10초, 0ㅊ 남았을 때 다트윈 애니메이션을 위해 켜지는 타이밍을 따로 조절
        }

    }
}
