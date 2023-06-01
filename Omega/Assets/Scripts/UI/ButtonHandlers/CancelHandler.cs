using Omega.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Omega.UI
{
    public class CancelHandler : MonoBehaviour
    {
        [SerializeField] private Selectable selectableToFocus;
        [SerializeField] private GameObject gameobjectToDisable;
        private PlayerIdentifier playerIdentifier;

        private InputAction cancelAction;
        private GameObject lastSelectedGameObject;

        public bool cannotCancel = false;

        private CanvasGroup playerHUD;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            playerHUD = FindObjectOfType<RoundHandler>().playerHUD;
        }

        private void OnEnable()
        {
            //cannotCancel = false;
            cancelAction = new InputAction("Cancel", InputActionType.Button, "<Gamepad>/buttonEast");
            cancelAction.performed += OnCancelPressed;
            cancelAction.Enable();
            lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        }

        private void OnDisable()
        {
            cancelAction.Disable();
            cancelAction.performed -= OnCancelPressed;
            if (lastSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
            }
        }

        private void OnCancelPressed(InputAction.CallbackContext context)
        {
            if (cannotCancel) return;
            if (playerIdentifier.currentAttack != null && !playerIdentifier.currentAttack.currentlySelectingPlayer) return;
            if (playerIdentifier.currentHeal != null && !playerIdentifier.currentHeal.currentlySelectingPlayer) return;
            if (playerIdentifier.isAttacking) playerIdentifier.currentAttack = null;
            playerHUD.interactable = true;
            playerHUD.alpha = 1;
            ResetUI();

        }

        public void ResetUI()
        {
            lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (gameobjectToDisable != null)
            {
                gameobjectToDisable.SetActive(false);
            }
            if (selectableToFocus != null)
            {
                EventSystem.current.SetSelectedGameObject(selectableToFocus.gameObject);

            }
        }
    }
}
