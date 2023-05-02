using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;

namespace Omega.UI
{
    public class HealingButtonHandler : ActionButtonHandler
    {
        public Dice dice;

        private PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }

        private void OnEnable()
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            if(playerEnergy.energy < dice.cost)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }
        }
        
        public void ButtonPressed()
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            int extraHealth = dice.roll();
            Health playerHealth = playerIdentifier.currentPlayer.GetComponent<Health>();
            if (playerHealth.health + extraHealth >= playerHealth.maxHealth)
            {
                playerHealth.health = playerHealth.maxHealth;
            }
            else
            {
                playerHealth.AddHealth(extraHealth);
            }

            playerEnergy.energy -= dice.cost;
        }
    }
}