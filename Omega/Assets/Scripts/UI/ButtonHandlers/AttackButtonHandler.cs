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

        public void ButtonPressed()
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            int damage = dice.roll();
            List<GameObject> attackablePlayers = new List<GameObject>();
            playerEnergy.energy -= dice.cost;
            foreach (GameObject item in playerIdentifier.playerIndex)
            {
                if(item != playerIdentifier.currentPlayer)
                {
                    item.GetComponent<Selectable>().enabled = true;
                    item.GetComponent<Outline>().enabled = true;
                    item.GetComponent<Outline>().OutlineColor = Color.white;
                    attackablePlayers.Add(item);
                }

            }
            EventSystem eventSystem = EventSystem.current;
            eventSystem.SetSelectedGameObject(attackablePlayers[0]);
            Debug.Log(attackablePlayers[0]);
            attackablePlayers[0].GetComponent<Outline>().OutlineColor = Color.red;
            //PlayerSelectionHandler playerSelectionHandler = attackablePlayers[0].GetComponent<PlayerSelectionHandler>();
            //playerSelectionHandler.ChangeSelection();

        }
    }
}