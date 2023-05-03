using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Omega.UI
{
    public class AttackButtonHandler : ActionButtonHandler
    {
        public Dice dice;

        private PlayerIdentifier playerIdentifier;

        private int currentDamage;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
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
            eventSystem.SetSelectedGameObject(attackablePlayers[playerIdentifier.currentPlayerIndex]);

            PlayerSelectionHandler playerSelection = FindObjectOfType<PlayerSelectionHandler>();
            playerSelection.GetCurrentAction(this);
        }

        private void EnableBaseSelection(List<GameObject> attackablePlayers)
        {
            foreach (GameObject item in playerIdentifier.playerIndex)
            {
                if (item != playerIdentifier.currentPlayer)
                {
                    item.GetComponent<Selectable>().enabled = true;
                    item.GetComponent<Outline>().enabled = true;
                    item.GetComponent<Outline>().OutlineColor = Color.white;
                    attackablePlayers.Add(item);
                }

            }
        }

        private void DisableBaseSelection(List<GameObject> attackablePlayers)
        {
            foreach (GameObject item in playerIdentifier.playerIndex)
            {
                if (item != playerIdentifier.currentPlayer)
                {
                    if(item != null)
                    {
                        item.GetComponent<Selectable>().enabled = false;
                        item.GetComponent<Outline>().enabled = false;
                        attackablePlayers.Add(item);
                    }
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
            }
        }
    }
}