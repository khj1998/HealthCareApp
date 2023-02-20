using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BannerAdsCanvasSetting : MonoBehaviour
{
    [SerializeField]
    private GameObject adsCanvas;
    // Start is called before the first frame update
    void Start()
    {
        adsCanvas = GameObject.Find("ADAPTIVE(Clone)");
        
        if (adsCanvas != null)
        {
            adsCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
        }
    }
}