using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOMA;
namespace SOMA.UI
{
    public class SetNoAdsUI : MonoBehaviour
    {

        [SerializeField]
        private RectTransform[] viewCanvases;

        public void setTopMargin()
        {
            int len = viewCanvases.Length;
            for (int i = 0; i < len; i++)
            {
                viewCanvases[i].offsetMax = new Vector2(viewCanvases[i].offsetMax.x, 0);
            }
            Debug.Log("마진을 제거합니다");
        }

    }
}
