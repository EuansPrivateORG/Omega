using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.UI;

namespace Omega.Status
{
    public class Energy : MonoBehaviour
    {
        [HideInInspector] public int energy;

        private EnergyBar energyBar;

        private void Awake()
        {
            energyBar = FindObjectOfType<EnergyBar>();
        }

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
            energyBar.UpdateSegments();
        }

        public void SpendEnergy(int loss)
        {
            energy -= loss;
            energyBar.UpdateSegments();
        }

        public void SetEnergy(int amount, bool start)
        {
            energy = amount;
            if(!start)
            {
                energyBar.UpdateSegments();
            }
        }
    }
}