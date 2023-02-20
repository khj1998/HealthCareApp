using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOMA.Fight;

namespace SOMA.UI
{
    public class HpBar : MonoBehaviour
    {

        public float gauge;
        [SerializeField]
        private RectTransform GaugeBar;
        [SerializeField]
        private GameObject model;

        Camera camera;
        // Start is called before the first frame update
        void Start()
        {
            camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            //GaugeBar.anchoredPosition = new Vector2(fighter.hp * 4,0);
            GaugeBar.anchoredPosition = new Vector3(gauge * 4,0,0);
            //transform.position = camera.WorldToScreenPoint(model.position + new Vector3(0,1f,0));
        }
    }
}
