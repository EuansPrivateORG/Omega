using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using TMPro;
using Unity.Mathematics;

namespace Omega.UI
{
    public class HealingButtonHandler : ActionButtonHandler
    {
        public Dice dice;

        private PlayerIdentifier playerIdentifier;
        [SerializeField] public GameObject healingNumbersPrefab;

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
            Health playerHealth = playerIdentifier.currentPlayer.GetComponent<Health>();

            int extraHealth = dice.roll();
            if (playerHealth.currentHealth + extraHealth >= playerHealth.maxHealth)
            {
                playerHealth.currentHealth = playerHealth.maxHealth;
            }
            else
            {
                playerHealth.AddHealth(extraHealth);
            }
            GameObject numbersPrefab = Instantiate(healingNumbersPrefab, playerIdentifier.currentPlayer.transform.position, quaternion.identity);
            numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
            NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
            numbersDisplay.SpawnNumbers(extraHealth);

            playerEnergy.SpendEnergy(dice.cost);

            playerIdentifier.NextPlayer();
        }
    }
}