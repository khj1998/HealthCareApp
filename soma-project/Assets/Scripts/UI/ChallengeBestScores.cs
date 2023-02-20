using UnityEngine;
using TMPro;
using SOMA.Managers;

namespace SOMA
{
    public class ChallengeBestScores : MonoBehaviour
    {
        public TextMeshProUGUI SquatBestScoreText; 
        public TextMeshProUGUI PushUpBestScoreText;

        private void Update() 
        {
            SquatBestScoreText.text = DataManager.Instance.ChallengeScoresTable.SquatBestScore.ToString();
            PushUpBestScoreText.text = DataManager.Instance.ChallengeScoresTable.PushUpBestScore.ToString();
        }
    }
}
