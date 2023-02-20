using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOMA.Fight
{
    public class Fighter : MonoBehaviour
    {
        
        public float hp;
        public float attackPower;
        [SerializeField]
        private Animator animator;
        private int aniNum;
        [SerializeField]
        private GameObject myHpBar;

        private float attackPowerGague = 0;
        private float attackPowerGaugeLimit = 6; //3초 충전이 최대 게이지  1점 스쿼트 -> 6초    3점 스쿼트 -> 2초

        void Start()
        {
            animator.SetInteger("state", 0);
        }

        void Update()
        {
            myHpBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 8f, 0));
        }

        public void attack(Fighter enemy)
        {
            aniNum = 2;
            animator.SetInteger("state", aniNum);
            enemy.hp -= attackPowerGague * attackPower;
            attackPowerGague = 0;
            //타격 파티클 생성
            //타격 사운드
        }
        public void ChargingGauge(float level) //level => 자세 점수 1~3
        { 
            aniNum = 1;
            animator.SetInteger("state", aniNum);
            animator.SetFloat("chargingSpeed", level * 0.1f + 0.1f);
            attackPowerGague += level * Time.deltaTime; //자세점수 * 초만큼 게이지가 찬다
            Debug.Log(attackPowerGague);
            
            if (attackPowerGague > attackPowerGaugeLimit)
            {
                attackPowerGague = attackPowerGaugeLimit;
            }
            
        }
    }
}
