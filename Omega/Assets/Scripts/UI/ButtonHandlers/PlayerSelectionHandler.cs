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

        private AttackButtonHandler currentAttack;

        private HealingButtonHandler currentHeal;

        private TurnTimer turnTimer;

        private CanvasGroup playerHUD;

        private PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            eventSystem = FindObjectOfType<EventSystem>();
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

            if(currentAttack != null)
            {
                currentAttack.RollDice(eventSystem.currentSelectedGameObject);

                playerHUD.interactable = false;
                playerHUD.alpha = 0;
                turnTimer.SetTimeOff();

                currentAttack = null;
            }
            else
            {
                currentHeal.RollDice(eventSystem.currentSelectedGameObject);

                playerHUD.interactable = false;
                playerHUD.alpha = 0;
                turnTimer.SetTimeOff();

                currentHeal = null;
            }
        }



        private void Update()
        {
            if(eventSystem.currentSelectedGameObject == gameObject && currentAttack != null)
            {
                GetComponentInChildren<Outline>().OutlineColor = Color.red;
            }
            else if(eventSystem.currentSelectedGameObject == gameObject && currentAttack == null)
            {
                GetComponentInChildren<Outline>().OutlineColor = Color.green;
            }
            else
            {
                GetComponentInChildren<Outline>().OutlineColor = Color.white;
            }
        }

        public void GetCurrentAction(AttackButtonHandler attack, HealingButtonHandler heal)
        {
            if(attack == null)
            {
                currentAttack = null;
                currentHeal = heal;
            }
            else
            {
                currentAttack = attack;
                currentHeal = null;
            }
        }
    }
}
