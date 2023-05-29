using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Omega.UI
{
    public class CardButtonHandler : MonoBehaviour
    {
        EventSystem eventSystem;

        [HideInInspector] public GameObject currentCardHighlight = null;

        private CardTween cardTween;

        private PlayerInput playerInput;

        private bool hasEnabledInput = false;


        private void Awake()
        {
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
    }
}