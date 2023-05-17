using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using TMPro;
using Unity.Mathematics;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Omega.Combat;

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
        private CanvasGroup playerHUD;
        private CancelHandler cancelHandler;
        private GameObject nextAvailablePlayer;
        private GameObject playerToHeal;

        [HideInInspector] public bool currentlySelectingPlayer = false;
        private bool healedMax = false;
        private int oldCurrentHealth;
        private NumberRoller numberRoller;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            diceSpawner = FindObjectOfType<DiceSpawner>();
            playerHUD = FindObjectOfType<RoundHandler>().playerHUD;
            cancelHandler = GetComponent<CancelHandler>();
            numberRoller = FindObjectOfType<NumberRoller>();
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

        private void OnDisable()
        {
            List<GameObject> healablePlayers = new List<GameObject>();
            DisableBaseSelection(healablePlayers);
        }

        public void ButtonPressed()
        {
            playerIdentifier.isAttacking = false;

            foreach(GameObject player in playerIdentifier.playerIndex)
            {
                player.GetComponent<PlayerSelectionHandler>().GetCurrentAction(null, this);
            }

            List<GameObject> healablePlayers = new List<GameObject>();

            EnableBaseSelection(healablePlayers);

            EventSystem eventSystem = EventSystem.current;

            eventSystem.SetSelectedGameObject(nextAvailablePlayer);
        }

        private void EnableBaseSelection(List<GameObject> healablePlayers)
        {
            currentlySelectingPlayer = true;
            playerHUD.alpha = 0.25f;
            cancelHandler.cannotCancel = true;
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                PlayerSelectionHandler playerSelectionHandler = player.GetComponent<PlayerSelectionHandler>();
                playerSelectionHandler.enabled = true;
            }

            bool foundNextPlayer = false;

            for (int i = 0; i < playerIdentifier.turnOrderIndex.Count; i++)
            {
                GameObject playerObject = playerIdentifier.turnOrderIndex[i];

                if (!playerObject.GetComponent<Health>().isDead)
                {
                    playerObject.GetComponent<PlayerSelectionHandler>().enabled = true;
                    playerObject.GetComponent<Selectable>().enabled = true;
                    playerObject.GetComponentInChildren<Outline>().enabled = true;
                    playerObject.GetComponentInChildren<Outline>().OutlineColor = Color.white;
                    healablePlayers.Add(playerObject);

                    if (!foundNextPlayer)
                    {
                        nextAvailablePlayer = playerObject;
                        foundNextPlayer = true;
                    }
                }
            }

            if (!foundNextPlayer)
            {
                nextAvailablePlayer = null;
            }
        }

        private void DisableBaseSelection(List<GameObject> healablePlayers)
        {
            currentlySelectingPlayer = false;
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                PlayerSelectionHandler playerSelectionHandler = player.GetComponent<PlayerSelectionHandler>();
                playerSelectionHandler.enabled = false;

            }
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                if (player != null)
                {
                    player.GetComponent<PlayerSelectionHandler>().enabled = false;
                    player.GetComponent<Selectable>().enabled = false;
                    player.GetComponentInChildren<Outline>().enabled = false;
                    healablePlayers.Add(player);
                }
            }

        }


        public void RollDice(GameObject toHeal)
        {
            playerIdentifier.currentHeal = this;
            playerToHeal = toHeal;
            diceSpawner.ActivateDice(heal);

            List<GameObject> healablePlayers = new List<GameObject>();
            DisableBaseSelection(healablePlayers);
        }

        public void PerformHealing(int extraHealth)
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            playerEnergy.SpendEnergy(heal.cost);

            Health playerHealth = playerToHeal.GetComponent<Health>();

            if (playerHealth.currentHealth + extraHealth >= playerHealth.maxHealth)
            {
                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].pointsHealed += (playerHealth.maxHealth - playerHealth.currentHealth);

                oldCurrentHealth = playerHealth.currentHealth;
                playerHealth.currentHealth = playerHealth.maxHealth;
                healedMax = true;
                Debug.Log(playerHealth.maxHealth - oldCurrentHealth + " Health Healed");
            }
            else
            {
                playerHealth.AddHealth(extraHealth);
                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].pointsHealed += extraHealth;
                healedMax = false;
                Debug.Log(extraHealth + " Health Healed");
            }

            if (healedMax)
            {
                SpawnHealingNumbers(playerHealth.maxHealth - oldCurrentHealth);
            }
            else
            {
                SpawnHealingNumbers(extraHealth);
            }


            StartCoroutine(DelayNextTurn());
        }

        private void SpawnHealingNumbers(int healthHealed)
        {
            int minColour = heal.minDamageFromDice();
            int maxColour = heal.maxDamageFromDice();

            GameObject numbersPrefab = Instantiate(healingNumbersPrefab, playerToHeal.transform.position, quaternion.identity);

            numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = GetColorOnGradient(healthHealed, minColour, maxColour, colourGradient);
            NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
            numbersDisplay.SpawnNumbers(healthHealed);
        }

        private IEnumerator DelayNextTurn()
        {
            yield return new WaitForSeconds(1f);

            numberRoller.TurnOffNumberRoller();
            playerIdentifier.NextPlayer();

            yield return null;
        }

        public Color GetColorOnGradient(int value, int minValue, int maxValue, Gradient colorGradient)
        {
            float position = (float)(value - minValue) / (maxValue - minValue);
            return colorGradient.Evaluate(position);
        }
    }
}