using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOMA;
using UnityEngine.UI;
using TMPro;
using SOMA.Managers;

namespace SOMA
{
    public class NavBarBtn : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] deckObject;
        [SerializeField]
        private Image[] icons;
        [SerializeField]
        private TextMeshProUGUI[] navTexts;
        private List<string> _canvasList= new List<string>(){"challenge_canvas","routine_canvas","report_canvas","mypage_canvas"};

        void offAllCanvas()
        {
            for (int i = 0; i < 4; i++)
            {
                deckObject[i].SetActive(false);
                icons[i].color = new Color32(0,0,0, 100);
                navTexts[i].color = new Color32(0, 0, 0, 100);
            }
        }

        public void onSelectCanvas(int _seletNum)
        {
            offAllCanvas();
            deckObject[_seletNum].SetActive(true);
            icons[_seletNum].color = new Color32(0, 0, 0, 255);
            navTexts[_seletNum].color = new Color32(0, 0, 0, 255);

            #if !UNITY_EDITOR
            Debug.Log("캔버스 선택 아날리틱스");
            FirebaseManager.Instance.LogEvent(_canvasList[_seletNum]);
            #endif
        }
    }
}
