using Omega.Status;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Omega.Combat
{
    public class Projectile : MonoBehaviour
    {
        [HideInInspector]
        public int damage;
        private void OnTriggerEnter(Collider other)
        {
            other.GetComponent<Health>().TakeDamage(damage);
        }

        public void SetDamage(int damage)
        {
            this.damage = damage;
        }

    }
}
