using Omega.Actions;
using Omega.Core;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Omega.UI
{
    public class CardButtonHandler : MonoBehaviour
    {
        EventSystem eventSystem;

        [HideInInspector] public GameObject currentCardHighlight = null;

        private CardTween cardTween;

        private PlayerInput playerInput;

        private PlayerIdentifier playerIdentifier;

        private DrawCardHandler drawCardHandler;

        private bool hasEnabledInput = false;


        private void Awake()
        {
            drawCardHandler = FindObjectOfType<DrawCardHandler>();
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            eventSystem = EventSystem.current;
            cardTween = FindObjectOfType<CardTween>();
            playerInput = FindObjectOfType<PlayerInput>();
        }

        void Update()
        {
            if (eventSystem.currentSelectedGameObject == gameObject)
            {
                if(!hasEnabledInput)
                {
                    EnableInput();
                    hasEnabledInput = true;
                    SelectCard();
                }

                if(currentCardHighlight != null)
                {
                    currentCardHighlight.SetActive(true);
                }
            }
            else
            {
                if (hasEnabledInput)
                {
                    DisableInput();
                    hasEnabledInput = false;
                    UnSelectCard();
                }

                if (currentCardHighlight != null)
                {
                    currentCardHighlight.SetActive(false);
                }
            }
        }
        private void EnableInput()
        {
            playerInput.actions["ShuffleCards"].performed += OnLeftPressed;
            playerInput.actions["ShuffleCards"].Enable();
        }

        private void DisableInput()
        {
            playerInput.actions["ShuffleCards"].performed -= OnLeftPressed;
            playerInput.actions["ShuffleCards"].Disable();
        }

        public void OnLeftPressed(InputAction.CallbackContext context)
        {
            UnSelectCard();
            SelectCard();
        }

        public void OnButtonPressed()
        {
            cardTween.CardPlayed(transform.GetChild(0).gameObject);
        }

        public void SelectCard()
        {
            cardTween.CardUp(transform.GetChild(0).gameObject);
        }

        public void UnSelectCard()
        {
            cardTween.CardDown(transform.GetChild(0).gameObject);
        }

        public void CheckCards()
        {
            if(playerIdentifier.currentPlayer.GetComponent<PlayerCards>().cardsPlayed.Count >= 5)
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = drawCardHandler.healingButton;
                drawCardHandler.attackButton.navigation = newNav;
            }
            else
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = drawCardHandler.healingButton;
                newNav.selectOnLeft = GetComponent<Button>();
                drawCardHandler.attackButton.navigation = newNav;
            }
        }
    }
}