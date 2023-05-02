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

        public void ButtonPressed()
        {
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
        }
    }
}