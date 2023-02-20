using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOMA;
namespace SOMA.UI{
    public class NavBarBtn : MonoBehaviour
    {
        [SerializeField]
        private Canvas[] ViewCanvases;

        public enum NavBarCategory
        {
            MainView,
            DataView,
            Mypage
        }

        void offAllCanvas()
        {
            for (int i = 0; i < 3; i++)
            {
                ViewCanvases[i].enabled = false;
            }
        }
        public void OnSelectCanvas(int _seletNum)
        {
            offAllCanvas();
            ViewCanvases[_seletNum].enabled = true;
        }


    }
}
