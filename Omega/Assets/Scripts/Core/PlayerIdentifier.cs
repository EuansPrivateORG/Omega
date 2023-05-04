using Cinemachine;
using Omega.Status;
using Omega.UI;
using UnityEngine.UI;
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
        [HideInInspector]
        public List<GameObject> turnOrderIndex = new List<GameObject>();
        [HideInInspector]
        public List<GameObject> currentlyAlivePlayersInTurn = new List<GameObject>();
        [HideInInspector]
        public List<GameObject> currentlyAlivePlayers = new List<GameObject>();

        [SerializeField] public GameObject currentPlayer = null;
        public int energyGainPerTurn = 2;
        [HideInInspector]
        public int currentPlayerIndex = 0;
        TurnTimer turnTimer;

        private void Awake()
        {
            turnTimer = GetComponent<TurnTimer>();
        }

        public void SetupTurnOrderIndex()
        {
            turnOrderIndex.Clear();
            int numPlayers = playerIndex.Count;
            if (currentPlayerIndex < 0 || currentPlayerIndex >= numPlayers)
            {
                throw new ArgumentOutOfRangeException("currentPlayerIndex", "currentPlayerIndex is out of range.");
            }
            for (int i = currentPlayerIndex; i < numPlayers; i++)
            {
                turnOrderIndex.Add(playerIndex[i]);
            }
            for (int i = 0; i < currentPlayerIndex; i++)
            {
                turnOrderIndex.Add(playerIndex[i]);
            }
        }

        public void SetIndex(List<GameObject> playerList)
        {
            playerIndex = playerList;
            SetupTurnOrderIndex();
            SetupCurrentlyAlivePlayerIndex();

            currentPlayer = currentlyAlivePlayers[0];
            currentPlayer.GetComponent<Energy>().GainEnergy(energyGainPerTurn);
        }


        public void NextPlayer()
        {
            SetupTurnOrderIndex();
            SetupCurrentlyAlivePlayerIndex();

            currentPlayerIndex++;
            if (currentPlayerIndex >= currentlyAlivePlayers.Count)
            {
                currentPlayerIndex = 0;
            }
            UpdatePlayerIcon();
            currentPlayer = currentlyAlivePlayers[currentPlayerIndex];

            currentPlayer.GetComponent<Energy>().GainEnergy(energyGainPerTurn);
            CameraHandler cameraHandler = FindObjectOfType<CameraHandler>();
            TurnTransition turnTransition = FindObjectOfType<TurnTransition>();
            StartCoroutine(turnTransition.FadeOutHUD());
            cameraHandler.SwitchCamera(currentPlayer.GetComponentInChildren<CinemachineVirtualCamera>());
            StartCoroutine(DelayedHUDFadeIn(turnTransition));
            turnTimer.ResetTimer();

            CancelHandler handler = FindObjectOfType<CancelHandler>();
            if(handler != null) 
            {
                handler.ResetUI();
            }


        }

        private IEnumerator DelayedHUDFadeIn(TurnTransition turnTransition)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(turnTransition.FadeInHUD());

        }

        public void SetNextSelectedObjectInEventSystem(EventSystem eventSystem)
        {
            int currentIndex = currentlyAlivePlayers.IndexOf(currentPlayer);

            for (int i = 1; i < currentlyAlivePlayers.Count; i++)
            {
                int nextIndex = (currentIndex + i) % currentlyAlivePlayers.Count;

                if (currentlyAlivePlayers[nextIndex] != currentPlayer)
                {
                    eventSystem.SetSelectedGameObject(currentlyAlivePlayers[nextIndex]);
                    break;
                }
            }
        }

        private void SetupCurrentlyAlivePlayerIndex()
        {
            currentlyAlivePlayersInTurn.Clear();
            foreach (GameObject player in turnOrderIndex)
            {
                if (!player.GetComponent<Health>().isDead)
                {
                    currentlyAlivePlayersInTurn.Add(player);
                }
            }

            currentlyAlivePlayers.Clear();
            foreach (GameObject player in playerIndex)
            {
                if (!player.GetComponent<Health>().isDead)
                {
                    currentlyAlivePlayers.Add(player);
                }
            }
        }

        public int GetPlaceInIndex(GameObject obj)
        {
            for (int i = 0; i < currentlyAlivePlayersInTurn.Count; i++)
            {
                if(obj == currentlyAlivePlayersInTurn[i])
                {
                    return i;
                }
            }
            return 0;
        }

        private void UpdatePlayerIcon()
        {
            PlayerSpawnHandler playerSpawner = FindObjectOfType<PlayerSpawnHandler>();
            for (int i = 0; i < playerSpawner.playerImageList.Count; i++)
            {
                if (i  == currentPlayerIndex)
                {
                    playerSpawner.playerImageList[i].GetComponent<Image>().enabled = true;
                }
                else
                {
                    playerSpawner.playerImageList[i].GetComponent<Image>().enabled = false;
                }
            }
        }
    }
}
