using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Status
{
    public class Energy : MonoBehaviour
    {
        [HideInInspector] public int energy;

        private void Update()
        {
            if(energy > 16)
            {
                energy = 16;
            }
        }

        public void GainEnergy(int addition)
        {
            energy += addition;
            Debug.Log("Energy Gained " + addition);
        }

        public void SpendEnergy(int loss)
        {
            energy -= loss;
            //if (energy <= 0) energy = 0;
        }
    }
}