using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Status
{
    public class Energy : MonoBehaviour
    {
        [HideInInspector] public int energy;

        public void GainEnergy(int addition)
        {
            energy += addition;
        }

        public void SpendEnergy(int loss)
        {
            energy -= loss;
            //if (energy <= 0) energy = 0;
        }
    }
}