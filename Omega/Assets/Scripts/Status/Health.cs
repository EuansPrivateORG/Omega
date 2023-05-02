using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Status
{
    public class Health : MonoBehaviour
    {
        [HideInInspector] public int health;

        [HideInInspector] public int maxHealth;

        public void AddHealth(int addition)
        {
            health += addition;
        }

        public void TakeDamage(int loss)
        {
            health -= loss;
        }
    }
}