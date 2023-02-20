using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BannerAdsSize : MonoBehaviour
{
    public GameObject adsCanvas;
    public CanvasScaler canvasScaler;
    // Start is called before the first frame update
    void Start()
    {
        adsCanvas = GameObject.Find("ADAPTIVE(Clone)");
        canvasScaler = adsCanvas.GetComponent<CanvasScaler>();
        canvasScaler.referenceResolution = new Vector2(1080, 1920);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
