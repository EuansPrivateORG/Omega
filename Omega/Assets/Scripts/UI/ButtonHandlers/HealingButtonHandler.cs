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
        public Actions.Action dice;

        private PlayerIdentifier playerIdentifier;
        [SerializeField] public GameObject healingNumbersPrefab;
        [SerializeField] public Gradient colourGradient;

        private ScoreHandler scoreHandler;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
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
                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].pointsHealed += (playerHealth.maxHealth - playerHealth.currentHealth);

                playerHealth.currentHealth = playerHealth.maxHealth;
            }
            else
            {
                playerHealth.AddHealth(extraHealth);
                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].pointsHealed += extraHealth;
            }
            int minColour = dice.minimumRoll;
            int maxColour = dice.maximumRoll;
            GameObject numbersPrefab = Instantiate(healingNumbersPrefab, playerIdentifier.currentPlayer.transform.position, quaternion.identity);
            numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = GetColorOnGradient(extraHealth, minColour, maxColour, colourGradient);
            NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
            numbersDisplay.SpawnNumbers(extraHealth);

            playerEnergy.SpendEnergy(dice.cost);

            playerIdentifier.NextPlayer();
        }

        public Color GetColorOnGradient(int value, int minValue, int maxValue, Gradient colorGradient)
        {
            float position = (float)(value - minValue) / (maxValue - minValue);
            return colorGradient.Evaluate(position);
        }
    }
}