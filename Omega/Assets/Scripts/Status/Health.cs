using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Core;

namespace Omega.Status
{
    public class Health : MonoBehaviour
    {
        [HideInInspector] public int currentHealth;

        [HideInInspector] public int maxHealth;

        [HideInInspector] public bool isDead = false;

        public void AddHealth(int addition)
        {
            currentHealth += addition;
        }

        public void TakeDamage(int loss)
        {
            currentHealth -= loss;

            if(currentHealth <= 0)
            {
                isDead = true;
            }
        }

        public void SetHealth()
        {
            currentHealth = maxHealth;
        }
    }
}