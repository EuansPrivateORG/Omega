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

        private InputAction leftAction;

        private bool hasEnabledInput = false;


        private void Awake()
        {
            eventSystem = EventSystem.current;
            cardTween = FindObjectOfType<CardTween>();
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
            Debug.Log("Input Enabled");
            leftAction = new InputAction("ShuffleCards", InputActionType.Button, "<Gamepad>/dpad/left, <Gamepad>/leftStick/left"); 
            leftAction.performed += OnLeftPressed;
            leftAction.Enable();
        }

        private void DisableInput()
        {
            Debug.Log("Input Disabled");
            leftAction.Disable();
            leftAction.performed -= OnLeftPressed;
        }

        public void OnLeftPressed(InputAction.CallbackContext context)
        {
            Debug.Log("left Pressed");
            UnSelectCard();
            SelectCard();
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