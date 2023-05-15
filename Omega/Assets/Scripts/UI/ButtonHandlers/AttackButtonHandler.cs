using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using Unity.Mathematics;
using TMPro;
using Omega.Combat;
using UnityEditor.Timeline.Actions;
using UnityEngine.InputSystem;

namespace Omega.UI
{
    public class AttackButtonHandler : ActionButtonHandler
    {
        public PlayerAction attack;

        private PlayerIdentifier playerIdentifier;

        [HideInInspector]
        public int currentDamage;

        private GameObject nextAvailablePlayer;
        [SerializeField] public GameObject damageNumbersPrefab;
        [SerializeField] public Gradient colourGradient;

        private ScoreHandler scoreHandler;
        private DiceSpawner diceSpawner;
        private GameObject playerToDamage;
        public List<GameObject> attackablePlayers = new List<GameObject>();

        public CanvasGroup playerHUD = null;

        private CancelHandler cancelHandler;

        public bool currentlySelectingPlayer = false;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            diceSpawner = FindObjectOfType<DiceSpawner>();
            playerHUD = FindObjectOfType<RoundHandler>().playerHUD;
            cancelHandler = GetComponent<CancelHandler>();

        }

        private void OnEnable()
        {

            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            if (playerEnergy.energy < attack.cost)
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
            List<GameObject> attackablePlayers = new List<GameObject>();
            DisableBaseSelection(attackablePlayers);
        }

        public void ButtonPressed()
        {
            playerIdentifier.isAttacking = true;

            PlayerSelectionHandler playerSelection = FindObjectOfType<PlayerSelectionHandler>();

            List<GameObject> attackablePlayers = new List<GameObject>();

            playerSelection.GetCurrentAction(this);

            EnableBaseSelection(attackablePlayers);

            EventSystem eventSystem = EventSystem.current;

            eventSystem.SetSelectedGameObject(nextAvailablePlayer);

        }

        private void EnableBaseSelection(List<GameObject> attackablePlayers)
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

                if (playerObject != playerIdentifier.currentPlayer && !playerObject.GetComponent<Health>().isDead)
                {
                    playerObject.GetComponent<Selectable>().enabled = true;
                    playerObject.GetComponentInChildren<Outline>().enabled = true;
                    playerObject.GetComponentInChildren<Outline>().OutlineColor = Color.white;
                    attackablePlayers.Add(playerObject);

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

        private void DisableBaseSelection(List<GameObject> attackablePlayers)
        {
            currentlySelectingPlayer = false;
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
            PlayerSelectionHandler playerSelectionHandler = player.GetComponent<PlayerSelectionHandler>();
            playerSelectionHandler.enabled = false;

            }
            foreach (GameObject item in playerIdentifier.playerIndex)
            {
                if(item != null)
                {
                    item.GetComponent<Selectable>().enabled = false;
                    item.GetComponentInChildren<Outline>().enabled = false;
                    attackablePlayers.Add(item);
                }
            }

        }

        public void PerformAttack(int damageToDeal)
        {
            Debug.Log(damageToDeal);
            damageToDeal += attack.rollBonus;
            currentDamage = damageToDeal;

            int minColour = attack.minDamageFromDice();
            int maxColour = attack.maxDamageFromDice();


            playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().SpawnProjectile(damageToDeal, playerToDamage, minColour, maxColour, this);

            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();
            playerEnergy.SpendEnergy(attack.cost);
            scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].damageDealt += damageToDeal;
            
        }

        public void RollDice(GameObject toDamage)
        {
            playerIdentifier.currentAttack = this;
            playerToDamage = toDamage;
            diceSpawner.ActivateDice(attack);

            List<GameObject> attackablePlayers = new List<GameObject>();
            DisableBaseSelection(attackablePlayers);
        }

        public void SpawnDamageNumbers(GameObject toDamage, int minColour, int maxColour)
        {
            GameObject numbersPrefab = Instantiate(damageNumbersPrefab, toDamage.transform.position, quaternion.identity);
            numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = GetColorOnGradient(currentDamage, minColour, maxColour, colourGradient);
            NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
            numbersDisplay.SpawnNumbers(currentDamage);
        }

        public Color GetColorOnGradient(int value, int minValue, int maxValue, Gradient colorGradient)
        {
            float position = (float)(value - minValue) / (maxValue - minValue);
            return colorGradient.Evaluate(position);
        }
    }
}