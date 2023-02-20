using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOMA.Fight;
using SOMA.UI;

namespace SOMA.Fight
{
    public class FightManager : MonoBehaviour
    {

        public Fighter player;
        public Fighter enemy;

        [SerializeField]
        private HpBar playerHpBar;
        [SerializeField]
        private HpBar enemyHpBar;

        public enum State
        {
            inCamera,
            showPlayer,
            ready,
            inFight,
            end,
            waitState
        };
        
        void Start()
        {
            player.hp = 100;
            enemy.hp = 100;
            player.attackPower = 2; //충전한 게이지 * atkPower
        }

        // Update is called once per frame
        void Update()
        {
            playerHpBar.gauge = player.hp;
            enemyHpBar.gauge = enemy.hp;
            
            if (Input.GetKey(KeyCode.Space) == true)
            {
                player.ChargingGauge(3f);
            }
            if (Input.GetKeyUp(KeyCode.Space) == true)
            {
                player.attack(enemy);
            }
        }
    }
}