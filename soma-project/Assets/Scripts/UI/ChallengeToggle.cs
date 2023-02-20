using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SOMA.UI
{
    public class ChallengeToggle : MonoBehaviour
    {
        public List<Toggle> ToggleList;
        public static bool IsSquatChallenge;
        public static bool IsPushUpChalllenge;

        void Update()
        {
            if (ToggleList[0].isOn)  // 스쿼트 챌린지 선택함.
            {
                IsSquatChallenge=true;
                IsPushUpChalllenge=false;
            }
            else // 푸시업 챌린지 선택함.
            {
                IsSquatChallenge=false;
                IsPushUpChalllenge=true;
            }
        }
}

}