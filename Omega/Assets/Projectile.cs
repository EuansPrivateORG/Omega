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

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }
        private void Update()
        {
            if (target == null) return;

            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * projectileSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider enemyCollider)
        {
            if (enemyCollider != instigator.GetComponentInChildren<Collider>())
            {
                Debug.Log(enemyCollider);
                target.GetComponent<Health>().TakeDamage(damage);
                Debug.Log(damage.ToString() + " Damage Dealt");
                attackButtonHandler.SpawnDamageNumbers(target, minColour, maxColour);
                Destroy(gameObject);
                playerIdentifier.NextPlayer();
            }
        }

        public void SetTarget(GameObject target, GameObject instigator, int damage, int min, int max, AttackButtonHandler origin)
        {
            this.target = target;
            this.instigator = instigator;
            this.damage = damage;
            minColour = min;
            maxColour = max;
            attackButtonHandler = origin;
        }

    }
}