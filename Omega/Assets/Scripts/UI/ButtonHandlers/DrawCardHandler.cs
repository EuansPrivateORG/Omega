using Omega.Actions;
using Omega.Core;
using Omega.Status;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Omega.UI
{
    public class DrawCardHandler : ActionButtonHandler
    {
        private CardHandler cardHandler;

        private PlayerIdentifier playerIdentifier;

        public Button healingButton;

        public Button skipButton;

        public Button attackButton;


        private void Awake()
        {
            cardHandler = FindObjectOfType<CardHandler>();
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }

        public void CheckEnergy()
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();
            if (playerEnergy.energy < cardHandler.cardCost || playerIdentifier.currentPlayer.GetComponent<PlayerCards>().cardsInHand.Count >= cardHandler.maxCards)
            {
                gameObject.GetComponent<Button>().interactable = false;

                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = skipButton;
                newNav.selectOnLeft = attackButton;
                healingButton.navigation = newNav;

                Navigation newNav2 = new Navigation();
                newNav2.mode = Navigation.Mode.Explicit;
                newNav2.selectOnLeft = healingButton;
                skipButton.navigation = newNav2;
            }
            else
            {
                Button button = GetComponent<Button>();

                gameObject.GetComponent<Button>().interactable = true;

                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = button;
                newNav.selectOnLeft = attackButton;
                healingButton.navigation = newNav;

                Navigation newNav2 = new Navigation();
                newNav2.mode = Navigation.Mode.Explicit;
                newNav2.selectOnLeft = button;
                skipButton.navigation = newNav2;
            }
        }

        public void ButtonPressed()
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();
            if(playerEnergy.energy >= cardHandler.cardCost)
            {
                cardHandler.DrawCard(playerIdentifier.currentPlayer, 1 , true, false);
                FindObjectOfType<InputSystemUIInputModule>().enabled = false;
                playerEnergy.SpendEnergy(cardHandler.cardCost);

            }
            CheckEnergy();
        }
    }
}