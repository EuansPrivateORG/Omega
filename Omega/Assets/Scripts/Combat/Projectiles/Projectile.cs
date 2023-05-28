using Cinemachine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        CardHandler cardHandler;

        private int bulletNum;


        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            numberRoller = FindObjectOfType<NumberRoller>();
            cardHandler = FindObjectOfType<CardHandler>();
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
                target.GetComponent<Health>().TakeDamage(damage);
                UnityEngine.Debug.Log(damage.ToString() + " Damage Dealt");
                enemyCollider.gameObject.GetComponentInChildren<AudioSource>().Play();
                attackButtonHandler.SpawnDamageNumbers(target, minColour, maxColour, false, damage);
                if (target.GetComponent<Health>().isDead)
                {
                    scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].playersKilled++;
                    cardHandler.DrawCard(playerIdentifier.currentPlayer, 1);
                }
                numberRoller.TurnOffNumberRoller();
                if(bulletNum == 0) playerIdentifier.NextPlayer();
                Destroy(transform.parent.gameObject);
            }
        }

        public void SetTarget(GameObject _target, GameObject _instigator, int _damage, int min, int max, AttackButtonHandler origin, int num)
        {
            target = _target;
            instigator = _instigator;
            damage = _damage;
            minColour = min;
            maxColour = max;
            attackButtonHandler = origin;
            bulletNum = num;
        }
    }
}