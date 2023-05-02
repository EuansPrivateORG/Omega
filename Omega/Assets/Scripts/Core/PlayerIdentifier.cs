using Cinemachine;
using Omega.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Omega.Core
{
    public class PlayerIdentifier : MonoBehaviour
    {
        [HideInInspector]
        public List<GameObject> playerIndex = new List<GameObject>();
        public GameObject currentPlayer = null;
        [HideInInspector]
        public int currentPlayerIndex = 0;
        TurnTimer turnTimer;

        private void Awake()
        {
            turnTimer = GetComponent<TurnTimer>();
        }

        public void SetIndex(List<GameObject> playerList)
        {
            playerIndex = playerList;
            currentPlayer = playerIndex[0];

        }

        public void NextPlayer()
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= playerIndex.Count)
            {
                currentPlayerIndex = 0;
            }
            currentPlayer = playerIndex[currentPlayerIndex];
            CameraHandler cameraHandler = FindObjectOfType<CameraHandler>();
            cameraHandler.SwitchCamera(currentPlayer.GetComponentInChildren<CinemachineVirtualCamera>());
            turnTimer.ResetTimer();
        }

        public void SetNextSelectedObjectInEventSystem(EventSystem eventSystem)
        {
            int currentIndex = playerIndex.IndexOf(currentPlayer);

            for (int i = 1; i < playerIndex.Count; i++)
            {
                int nextIndex = (currentIndex + i) % playerIndex.Count;

                if (playerIndex[nextIndex] != currentPlayer)
                {
                    eventSystem.SetSelectedGameObject(playerIndex[nextIndex]);
                    break;
                }
            }
        }
    }
}
