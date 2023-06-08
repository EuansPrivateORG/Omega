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

        public List<GameObject> playersToChooseFrom;

        void Start()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
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

        public void InitialNav()
        {
            // Set up the initial navigation for the Selectable component

            //if (playerIdentifier.flippedTurnOrder)
            //{
            //    var selectable = GetComponent<Selectable>();
            //    Navigation nav = selectable.navigation;
            //    nav.mode = Navigation.Mode.Explicit;
            //    nav.selectOnLeft = playersToChooseFrom[WrapIndex(GetPlaceInIndexNew(gameObject) + 1)].GetComponent<Selectable>();
            //    nav.selectOnRight = playersToChooseFrom[WrapIndex(GetPlaceInIndexNew(gameObject) - 1)].GetComponent<Selectable>();
            //    selectable.navigation = nav;
            //}
            var selectable = GetComponent<Selectable>();
            Navigation nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnRight = playersToChooseFrom[WrapIndex(GetPlaceInIndexNew(gameObject) + 1)].GetComponent<Selectable>();
            nav.selectOnLeft = playersToChooseFrom[WrapIndex(GetPlaceInIndexNew(gameObject) - 1)].GetComponent<Selectable>();
            selectable.navigation = nav;
        }

        void OnNavigatePerformed(InputAction.CallbackContext context)
        {
            // Handle the "Select on Left" and "Select on Right" actions
            var selectable = GetComponent<Selectable>();
            Navigation nav = selectable.navigation;

            if (playerIdentifier.isSelectingPlayer)
            {
                if (playerIdentifier.flippedTurnOrder)
                {
                    if (context.ReadValue<Vector2>().x < 0)
                    {
                        nav.selectOnRight = playersToChooseFrom[WrapIndex(GetPlaceInIndexNew(gameObject) - 1)].GetComponent<Selectable>();
                    }
                    else if (context.ReadValue<Vector2>().x > 0)
                    {
                        nav.selectOnLeft = playersToChooseFrom[WrapIndex(GetPlaceInIndexNew(gameObject) + 1)].GetComponent<Selectable>();
                    }
                }
                else
                {
                    if (context.ReadValue<Vector2>().x < 0)
                    {
                        nav.selectOnLeft = playersToChooseFrom[WrapIndex(GetPlaceInIndexNew(gameObject) - 1)].GetComponent<Selectable>();
                    }
                    else if (context.ReadValue<Vector2>().x > 0)
                    {
                        nav.selectOnRight = playersToChooseFrom[WrapIndex(GetPlaceInIndexNew(gameObject) + 1)].GetComponent<Selectable>();
                    }
                }
                selectable.navigation = nav;
            }
        }

        private int GetPlaceInIndexNew(GameObject obj)
        {
            for (int i = 0; i < playersToChooseFrom.Count; i++)
            {
                if (obj == playersToChooseFrom[i])
                {
                    return i;
                }
            }
            return 0;
        }



        int WrapIndex(int index)
        {
            // Helper function to wrap an index around the start and end of the list
            return (index + playersToChooseFrom.Count) % playersToChooseFrom.Count;
        }
    }
}