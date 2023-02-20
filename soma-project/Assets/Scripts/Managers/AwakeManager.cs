using UnityEngine;
using UnityEngine.SceneManagement;

public class AwakeManager : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene("StartScene");
    }
}
