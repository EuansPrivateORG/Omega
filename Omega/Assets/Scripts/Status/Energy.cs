using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Status
{
    public class Energy : MonoBehaviour
    {
        [HideInInspector] public int energy;

        public void AddHealth(int addition)
        {
            energy += addition;
        }

        public void TakeDamage(int loss)
        {
            energy -= loss;
        }
    }
}