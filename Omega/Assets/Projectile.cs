using Omega.Status;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Combat
{
    public class Projectile : MonoBehaviour
    {
        [HideInInspector]
        public int damage;

        public float projectileLifetime = 3f;
        public float projectileSpeed = 10f;

        GameObject instigator = null;
        GameObject target = null;


        private void Update()
        {
            if (target == null) return;

            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * projectileSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == target)
            {
                //Health targetHealth = target.GetComponent<Health>();
                //if (targetHealth != null)
                //{
                //    targetHealth.TakeDamage(damage);
                //}
                Destroy(gameObject);
            }
        }

        public void SetTarget(GameObject target, GameObject instigator, int damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
        }
    }
}
