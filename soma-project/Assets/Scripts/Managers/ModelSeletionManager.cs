using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.SceneManagement;
using SOMA.Managers;

namespace SOMA
{
    public class ModelSeletionManager : MonoBehaviour
{
    [SerializeField]
    private SimpleScrollSnap modelSnap;
    
    [SerializeField]
    private GameObject[] models;

    [SerializeField]
    private GameObject camera;

    [SerializeField]
    private GameObject pickBtn;
    [SerializeField]
    private GameObject buyBtn;
    [SerializeField]
    private TextMeshProUGUI buyBtnText;
    private Vector3 camPos;
    
    void Start()
    {
        modelSnap.StartingPanel = ModelData.Instance.selectedModel;
        camPos = new Vector3(modelSnap.StartingPanel * 4,0,-10);
        int modelLen = models.Length;
        for (int i = 0; i < modelLen; i++)
        {
            models[i].transform.localScale = new Vector3(4,4,4);
        }
        models[modelSnap.StartingPanel].transform.localScale = new Vector3(5,5,4);
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.position = Vector3.Lerp(camera.transform.position, camPos, 0.05f);
    }
    public void onMovedScroll()
    {
        //camera.transform.position = new Vector3(modelSnap.CenteredPanel * 4,0,-10);
        camPos = new Vector3(modelSnap.CenteredPanel * 4,0,-10);
        int modelLen = models.Length;
        for (int i = 0; i < modelLen; i++)
        {
            models[i].transform.localScale = new Vector3(4,4,4);
        }
        models[modelSnap.CenteredPanel].transform.localScale = new Vector3(5,5,4);
        setBtns();
        Debug.Log(modelSnap.CenteredPanel);
    }
    void setBtns()
    {
        if (ModelData.Instance.unlockModelList[modelSnap.CenteredPanel] == 1) //사용 가능
        {
            pickBtn.SetActive(true);
            buyBtn.SetActive(false);
        }
        else
        {
            pickBtn.SetActive(false);
            buyBtn.SetActive(true);
            buyBtnText.text = $"구매\n{ModelData.Instance.modelPrices[modelSnap.CenteredPanel]}원";
        }
    }

    public void onClickPickBtn()
    {
        int selectedNumber = modelSnap.CenteredPanel;

        ModelData.Instance.selectedModel = selectedNumber;
        DataManager.Instance.UpdateModelNumber(selectedNumber);

        if (SOMA.firebaseDB.ConnectFirebaseDB.Instance.reference!=null)
		{	
            #if !UNITY_EDITOR
			SOMA.firebaseDB.ConnectFirebaseDB.Instance.UpdateModelNumber(SOMA.Managers.FirebaseManager.Instance.IdToken,selectedNumber);
            #endif
		}
        
        Debug.Log("선택한 모델번호를 업데이트 합니다.");
      
        SceneManager.LoadScene("MainScene");
    }

    public void onClickBuyBtn()
    {
        
    }
}

}
