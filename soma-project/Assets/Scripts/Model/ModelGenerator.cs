using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOMA.Managers;

namespace SOMA
{
    public class ModelGenerator : MonoBehaviour
{
    public GameObject[] models;
    GameObject nowModel;
    private int selectedModel;

    // 최초로 모델을 선택했다면, ModelData.Instance.selectedModel참조, 선택한 모델이 이미 있다면 ES3 데이터 참조하는 방식
    void OnEnable()
    {
        if (DataManager.Instance.UserInfosTable.SelectedModel == -1)
        {
            selectedModel = ModelData.Instance.selectedModel;
            //Instantiate(models[ModelData.Instance.selectedModel], this.transform.position, models[ModelData.Instance.selectedModel].transform.rotation);
            nowModelSelect(selectedModel);
        }
        else
        {
            selectedModel = DataManager.Instance.UserInfosTable.SelectedModel;
            nowModelSelect(selectedModel);
        }
    }

    void nowModelSelect(int selectedModel)
    {
        nowModel = (GameObject)Instantiate(models[selectedModel], this.transform.position, models[selectedModel].transform.rotation);
        nowModel.transform.parent = this.transform;
    }
}
}
