using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using SOMA.Managers;

namespace SOMA.Exit
{
    public class ExitFeature : MonoBehaviour
    {
        public GameObject QuitCanvas;
        private bool _isEscClicked;

        void Awake()
        {
            _isEscClicked=false;
        }

        void Update()
        {
            if (!DataManager.Instance.previewOn)
            {
                if (!_isEscClicked && Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("종료버튼 create");
                    _isEscClicked = true;
                    QuitCanvas.SetActive(true);
                }
                else if (_isEscClicked && Input.GetKeyDown(KeyCode.Escape))
                {
                    Debug.Log("종료 UI destroy");
                    _isEscClicked = false;
                    QuitCanvas.SetActive(false);
                }
            }
        }

        public void QuitChallenge()
        {
            _isEscClicked = false;
            QuitCanvas.SetActive(true);
        }

        public void GoBackToMainFromChallenge()
        {
            if (ChallengeManager.state ==ChallengeManager.State.guide || ChallengeManager.state ==ChallengeManager.State.inCamera ||ChallengeManager.state ==ChallengeManager.State.ready)
            {
                QuitCanvas.SetActive(false);
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                QuitCanvas.SetActive(false);
                ChallengeManager.state = ChallengeManager.State.report;
            }
        }


        public void QuitExercise()
        {
            _isEscClicked = false;
            QuitCanvas.SetActive(true);
        }

        public void Quit()    
        {
            QuitCanvas.SetActive(false);
            Application.Quit();
        }

        public void GoBackToMain()
        {
            QuitCanvas.SetActive(false);
            SceneManager.LoadScene("MainScene");
        }

        public void GoReportScene()
        {
            WorkOutManager.state = WorkOutManager.State.report;
            QuitCanvas.SetActive(false);
        }

        public void Continue()
        {
            QuitCanvas.SetActive(false);
        }
    }
}
