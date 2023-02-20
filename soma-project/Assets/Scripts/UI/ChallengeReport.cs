using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using SOMA.Managers;

namespace SOMA.UI
{
    public class ChallengeReport : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nowScore;
    [SerializeField]
    private TextMeshProUGUI bestScore;
    [SerializeField]
    private TextMeshProUGUI challengeTime;
    [SerializeField]
    private TextMeshProUGUI challengeExp;

    public GameObject ReportObject;
    
	private int minutes;
	private int seconds;

    void Awake()
    {
        showReport();
    }

    private void showReport()
    {
        int totalChallengeTime = DataManager.Instance.ChallengeReportDataStorage.ChallengeExerTime;

        nowScore.text = DataManager.Instance.ChallengeReportDataStorage.NowScore.ToString();

        if (SOMA.UI.ChallengeToggle.IsSquatChallenge)
        {
            bestScore.text = DataManager.Instance.ChallengeScoresTable.SquatBestScore.ToString();
        }
        else if(SOMA.UI.ChallengeToggle.IsPushUpChalllenge)
        {
            bestScore.text = DataManager.Instance.ChallengeScoresTable.PushUpBestScore.ToString();
        }

        challengeTime.text = DataManager.Instance.ChallengeReportDataStorage.ChallengeExerTime.ToString()+"초";
        challengeExp.text = "+ "+DataManager.Instance.ChallengeReportDataStorage.NowChallengeExp.ToString();
    }
    
    public void GoToMain()
    {
        ReportObject.SetActive(false);
        SceneManager.LoadScene("MainScene");
    }
}

}
