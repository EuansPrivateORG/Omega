using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using TMPro;
using Unity.Mathematics;
using System;

namespace Omega.UI
{
    public class HealingButtonHandler : ActionButtonHandler
    {
        public PlayerAction heal;

        private PlayerIdentifier playerIdentifier;
        [SerializeField] public GameObject healingNumbersPrefab;
        [SerializeField] public Gradient colourGradient;

        private ScoreHandler scoreHandler;
        private DiceSpawner diceSpawner;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            diceSpawner = FindObjectOfType<DiceSpawner>();
        }

        private void OnEnable()
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            if(playerEnergy.energy < heal.cost)
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
            playerIdentifier.isAttacking = false;

            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            playerEnergy.SpendEnergy(heal.cost);

            RollDice(gameObject);
        }

        public void RollDice(GameObject toHeal)
        {
            diceSpawner.ActivateDice(heal);
        }

        public void PerformHealing()
        {
            Health playerHealth = playerIdentifier.currentPlayer.GetComponent<Health>();

            int extraHealth = 0;
            if (playerHealth.currentHealth + extraHealth >= playerHealth.maxHealth)
            {
                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].pointsHealed += (playerHealth.maxHealth - playerHealth.currentHealth);

                playerHealth.currentHealth = playerHealth.maxHealth;
            }
            else
            {
                playerHealth.AddHealth(extraHealth);
                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].pointsHealed += extraHealth;
            }

            int minColour = heal.minDamageFromDice();
            int maxColour = heal.maxDamageFromDice();

            GameObject numbersPrefab = Instantiate(healingNumbersPrefab, playerIdentifier.currentPlayer.transform.position, quaternion.identity);
            numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = GetColorOnGradient(extraHealth, minColour, maxColour, colourGradient);
            NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
            numbersDisplay.SpawnNumbers(extraHealth);

            playerIdentifier.NextPlayer();
        }

        public Color GetColorOnGradient(int value, int minValue, int maxValue, Gradient colorGradient)
        {
            float position = (float)(value - minValue) / (maxValue - minValue);
            return colorGradient.Evaluate(position);
        }
    }
}