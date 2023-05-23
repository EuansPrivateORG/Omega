using Cinemachine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Combat
{
    public class Projectile : MonoBehaviour
    {
        public int damage;

        public float projectileLifetime = 3f;
        public float projectileSpeed = 10f;

        GameObject instigator = null;
        GameObject target = null;

        int minColour;
        int maxColour;

        PlayerIdentifier playerIdentifier;
        AttackButtonHandler attackButtonHandler;
        ScoreHandler scoreHandler;
        NumberRoller numberRoller;

        private int bulletNum;


        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            numberRoller = FindObjectOfType<NumberRoller>();
        }
        private void Update()
        {
            if (target == null) return;

            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * projectileSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider enemyCollider)
        {
            if (enemyCollider != instigator.GetComponent<Collider>())
            {
                Debug.Log(enemyCollider.transform.parent.gameObject.name);
                target.GetComponent<Health>().TakeDamage(damage);
                Debug.Log(damage.ToString() + " Damage Dealt");
                enemyCollider.gameObject.GetComponentInChildren<AudioSource>().Play();
                attackButtonHandler.SpawnDamageNumbers(target, minColour, maxColour, false, damage);
                if (target.GetComponent<Health>().isDead)
                {
                    scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].playersKilled++;
                }
                numberRoller.TurnOffNumberRoller();
                if(bulletNum == 0) playerIdentifier.NextPlayer();
                Destroy(transform.parent.gameObject);
            }
        }

        public void SetTarget(GameObject target, GameObject instigator, int damage, int min, int max, AttackButtonHandler origin, int num)
        {
            this.target = target;
            this.instigator = instigator;
            this.damage = damage;
            minColour = min;
            maxColour = max;
            attackButtonHandler = origin;
            bulletNum = num;
        }



    }
}