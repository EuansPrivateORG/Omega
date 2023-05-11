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

        private InputAction attackAction;

        private AttackButtonHandler currentAction;

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

        private void OnPlayerPressed(InputAction.CallbackContext context)
        {
            if(currentAction != null)
            {
                currentAction.RollDice(eventSystem.currentSelectedGameObject);
                currentAction = null;
            }
        }

        private void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
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
