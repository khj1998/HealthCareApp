using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MyPage : MonoBehaviour
{
    public void onClickChageModelBtn()
    {
        SceneManager.LoadScene("SelectModel");
    }

    public void GoPrivacyPolicyURL()
    {
        Application.OpenURL("https://noble-magazine-1e6.notion.site/3deba9795db0487e8370e26e446e11c1");
    }
}
