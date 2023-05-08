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
using Cinemachine;
using TMPro;

namespace Omega.UI
{
    public class AttackButtonHandler : ActionButtonHandler
    {
        public Dice dice;

        private PlayerIdentifier playerIdentifier;

        private int currentDamage;

        private GameObject nextAvailablePlayer;
        [SerializeField] public GameObject damageNumbersPrefab;

        private ScoreHandler scoreHandler;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
        }

        private void OnEnable()
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            if (playerEnergy.energy < dice.cost)
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
            int damage = dice.roll();
            currentDamage = damage;

            List<GameObject> attackablePlayers = new List<GameObject>();

            EnableBaseSelection(attackablePlayers);

            EventSystem eventSystem = EventSystem.current;

            eventSystem.SetSelectedGameObject(nextAvailablePlayer);

            PlayerSelectionHandler playerSelection = FindObjectOfType<PlayerSelectionHandler>();
            playerSelection.GetCurrentAction(this);
        }

        private void EnableBaseSelection(List<GameObject> attackablePlayers)
        {
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

        public void PerformAttack(GameObject toDamage)
        {
            if(toDamage.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(currentDamage);
                Debug.Log(currentDamage.ToString() + " Damage Dealt");

                Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

                playerEnergy.SpendEnergy(dice.cost);

                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].damageDealt += currentDamage;
                if (health.isDead)
                {
                    scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].playersKilled++;
                }

                playerIdentifier.NextPlayer();
                GameObject numbersPrefab = Instantiate(damageNumbersPrefab, toDamage.transform.position, quaternion.identity);
                numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
                numbersDisplay.SpawnNumbers(currentDamage);
            }
        }
    }
}