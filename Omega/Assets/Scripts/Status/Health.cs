using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Status
{
    public class Health : MonoBehaviour
    {
        [HideInInspector] public int currentHealth;

        [HideInInspector] public int maxHealth;

        public void AddHealth(int addition)
        {
            currentHealth += addition;
        }

        public void TakeDamage(int loss)
        {
            currentHealth -= loss;
        }

        public void SetHealth()
        {
            currentHealth = maxHealth;
        }
    }
}