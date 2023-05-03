using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Omega.UI
{
    public class BaseSelectionHandlerOverwrite : MonoBehaviour
    {
        private PlayerIdentifier playerIdentifier;


        void Start()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();

            // Set up the initial navigation for the Selectable component
            var selectable = GetComponent<Selectable>();
            Navigation nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnLeft = playerIdentifier.currentlyAlivePlayers[WrapIndex(playerIdentifier.GetPlaceInIndex(gameObject) - 1)].GetComponent<Selectable>();
            nav.selectOnRight = playerIdentifier.currentlyAlivePlayers[WrapIndex(playerIdentifier.GetPlaceInIndex(gameObject) + 1)].GetComponent<Selectable>();
            selectable.navigation = nav;
        }



        void OnEnable()
        {
            // Subscribe to the "Select on Left" and "Select on Right" actions of the Input System
            InputSystemUIInputModule inputModule = FindObjectOfType<InputSystemUIInputModule>();
            inputModule.actionsAsset.FindAction("UI/Navigate").performed += OnNavigatePerformed;
        }



        void OnDisable()
        {
            // Unsubscribe from the Input System events
            InputSystemUIInputModule inputModule = FindObjectOfType<InputSystemUIInputModule>();
            if (inputModule != null )
            {
                inputModule.actionsAsset.FindAction("UI/Navigate").performed -= OnNavigatePerformed;
            }
        }



        void OnNavigatePerformed(InputAction.CallbackContext context)
        {
            // Handle the "Select on Left" and "Select on Right" actions
            var selectable = GetComponent<Selectable>();
            Navigation nav = selectable.navigation;
            if (context.ReadValue<Vector2>().x < 0)
            {
                nav.selectOnLeft = playerIdentifier.currentlyAlivePlayers[WrapIndex(playerIdentifier.GetPlaceInIndex(gameObject) - 1)].GetComponent<Selectable>();
            }
            else if (context.ReadValue<Vector2>().x > 0)
            {
                nav.selectOnRight = playerIdentifier.currentlyAlivePlayers[WrapIndex(playerIdentifier.GetPlaceInIndex(gameObject) + 1)].GetComponent<Selectable>();
            }
            selectable.navigation = nav;
        }



        int WrapIndex(int index)
        {
            // Helper function to wrap an index around the start and end of the list
            return (index + playerIdentifier.currentlyAlivePlayers.Count) % playerIdentifier.currentlyAlivePlayers.Count;
        }
    }
}