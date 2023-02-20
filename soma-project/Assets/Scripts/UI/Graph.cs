using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SOMA;

public class Graph : MonoBehaviour
{

    int[] data;
    public int yMax;


    [SerializeField]
    private GameObject[] bar;
    [SerializeField]
    private RectTransform[] barRect;
    [SerializeField]
    private TextMeshProUGUI xMaxText;
    [SerializeField]
    private TextMeshProUGUI xMeanText;
    [SerializeField]
    private TextMeshProUGUI yMaxText;
    [SerializeField]
    private TextMeshProUGUI yMeanText;

    private RectTransform Rect;
    [SerializeField]
    private HorizontalLayoutGroup layout;


    int xMax;
    int xMean;
    int yMean;

    private int nowDate;
    private const float _barMaxHeight = 712.8f;

    void Awake()
    {
        data = new int[7]{0,0,0,0,0,0,0};
        nowDate = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalDays;
    }
    
    void Start()
    {
        setData();

        Rect = GetComponent<RectTransform>();
        // (부모 넓이/막대기수) * 0.33 == spacing
        xMax = data.Length;
        xMean = (xMax  +1 )/2;
        yMean = yMax/2;

        //layout.spacing = (int)((int)rect.sizeDelta.x/xMax * 0.33f);
        
        xMaxText.text = $"{xMax}";
        xMeanText.text = $"{xMean}";
        yMaxText.text = $"{yMax}";
        yMeanText.text = $"{yMean}";

        int i = 0;

        for (i = 0; i < xMax; i++)
        {
            //막대기 높이 = (부모높이/데이터최대값)*데이터 값
            //barRect[i].sizeDelta = new Vector2(64, MathF.Min(_barMaxHeight,(rect.sizeDelta.y/yMax)*(data[i]*1.8f)));
            barRect[i].sizeDelta = new Vector2(64, MathF.Min(_barMaxHeight, (Rect.rect.height / yMax) * (data[i] * 1.8f) ));
            // max 높이 712.8
            bar[i].SetActive(true);
        }

        int j=0;

        for (j = i; j < 60; j++)
        {
            bar[j].SetActive(false);
        }
    }

    void setData()
    {
        /*int startIdx= DataManager.Instance.UserInfosTable.GraphStartIdx;
        int endIdx = DataManager.Instance.UserInfosTable.GraphEndIdx;

        if (endIdx!=-1)
        {
             for (int i=startIdx;i<=endIdx;i++)
            {
                data[i-startIdx]=DataManager.Instance.UserInfosTable.GraphDatas[i];
            }
        }*/
    }
}
