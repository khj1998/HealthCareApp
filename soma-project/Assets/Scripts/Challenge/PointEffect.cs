using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOMA;
using SOMA.UI;
using TMPro;

namespace SOMA
{
    public class PointEffect : MonoBehaviour
    {
        [SerializeField]
        private float waitTime;

        [SerializeField]
        private TextMeshProUGUI pointText;

        RectTransform rec;
        void Start()
        {
            rec = GetComponent<RectTransform>();
            int rx = UnityEngine.Random.Range(-150, 150);
            int ry = UnityEngine.Random.Range(-400, 400);
            if (rx < 0)
            {
                rx -= 150;
            }
            else
            {
                rx += 150;
            }
            rec.anchoredPosition = new Vector2(rx, ry);
            Destroy(gameObject, waitTime);
            pointText.text = $" +  {ChallengeManager.lastGetPoint}";
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
