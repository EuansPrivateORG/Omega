using Omega.Actions;
using Omega.Core;
using Omega.Status;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Omega.UI
{
    public class PlayerSelectionHandler : MonoBehaviour
    {
        EventSystem eventSystem;

        [HideInInspector] public InputAction attackAction;

        private AttackButtonHandler currentAction;

        private NumberRoller numberRoller;

        private TurnTimer turnTimer;

        private CanvasGroup playerHUD;

        private void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            numberRoller = FindObjectOfType<NumberRoller>();
            turnTimer = FindObjectOfType<TurnTimer>();
            playerHUD = FindObjectOfType<RoundHandler>().playerHUD;
        }
        private void OnEnable()
        {
            attackAction = new InputAction("Submit", InputActionType.Button, "<Gamepad>/buttonSouth");
            attackAction.performed += OnPlayerPressed;
            attackAction.Enable();
        }

        private void OnDisable()
        {
            attackAction.Disable();
            attackAction.performed -= OnPlayerPressed;
        }

        public void OnPlayerPressed(InputAction.CallbackContext context)
        {

            if(currentAction != null)
            {
                currentAction.RollDice(eventSystem.currentSelectedGameObject);

                numberRoller.rollers.gameObject.SetActive(true);
                numberRoller.StartRolling();
                numberRoller.AddBonusNumbers(currentAction.attack.rollBonus);
                playerHUD.interactable = false;
                playerHUD.alpha = 0;
                turnTimer.SetTimeOff();

                currentAction = null;
            }
        }



        private void Update()
        {
            if(eventSystem.currentSelectedGameObject == gameObject)
            {
                GetComponentInChildren<Outline>().OutlineColor = Color.red;
            }
            else
            {
                GetComponentInChildren<Outline>().OutlineColor = Color.white;
            }
        }

        public void GetCurrentAction(AttackButtonHandler action)
        {
            currentAction = action;
        }

    }
}
