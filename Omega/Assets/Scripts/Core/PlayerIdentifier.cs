using Cinemachine;
using Omega.Status;
using Omega.UI;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Omega.Actions;
using Omega.Combat;

namespace Omega.Core
{
    public class PlayerIdentifier : MonoBehaviour
    {
        public List<GameObject> playerIndex = new List<GameObject>();
        public List<GameObject> turnOrderIndex = new List<GameObject>();
        public List<GameObject> currentlyAlivePlayersInTurn = new List<GameObject>();
        public List<GameObject> currentlyAlivePlayers = new List<GameObject>(); 

        [HideInInspector] public AttackButtonHandler currentAttack;
        [HideInInspector] public HealingButtonHandler currentHeal;

        [SerializeField] public GameObject currentPlayer = null;
        public List<GameObject> currentPlayerWeapons = new List<GameObject>();

        public int energyGainPerTurn = 2;
        public int currentPlayerIndex = 0;
        TurnTimer turnTimer;
        private int playerWhoHasDied;
        [HideInInspector] public bool roundOver = false;
        [HideInInspector] public bool isAttacking = false;

        private PhysicalDiceCalculator physicalDiceCalculator;
        private CardSpawner cardSpawner;
        private DrawCardHandler drawCardHandler;

        private void Awake()
        {
            turnTimer = GetComponent<TurnTimer>();
            physicalDiceCalculator = GetComponent<PhysicalDiceCalculator>();
            cardSpawner = GetComponent<CardSpawner>();
            drawCardHandler = FindObjectOfType<DrawCardHandler>();
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

            UpdatePlayerIcon();

            cardSpawner.SpawnCards(currentPlayer.GetComponent<PlayerCards>().cardsInHand);
            
            drawCardHandler.CheckEnergy();

            playerWhoHasDied = playerIndex.Count + 1;

        }

        public void ResetIndex()
        {
            playerIndex.Clear();
            turnOrderIndex.Clear();
            currentlyAlivePlayers.Clear();
            currentlyAlivePlayersInTurn.Clear();
            currentPlayerIndex = 0; 

            currentPlayer = null;

            playerWhoHasDied = 0;
        }


        public void NextPlayer()
        {
            if (!roundOver)
            {
                physicalDiceCalculator.ClearDice();

                SetupTurnOrderIndex();

                SetupCurrentlyAlivePlayerIndex();

                CalculateCurrentPlayerIndex();

                UpdatePlayerIcon();

                SettingUpNextPlayer();

                //SetupCurrentPlayerWeapons(currentPlayer);

                CancelHandler handler = FindObjectOfType<CancelHandler>();
                if (handler != null)
                {
                    handler.ResetUI();
                }

                currentAttack = null;
                currentHeal = null;

                drawCardHandler.CheckEnergy();

                cardSpawner.ClearActiveCards();
                cardSpawner.SpawnCards(currentPlayer.GetComponent<PlayerCards>().cardsInHand);
            }
        }

        private void SettingUpNextPlayer()
        {
            currentPlayer = currentlyAlivePlayers[currentPlayerIndex];
            Debug.Log("This Players Turn: " + currentPlayer);

            currentPlayer.GetComponent<Energy>().GainEnergy(energyGainPerTurn);
            CameraHandler cameraHandler = FindObjectOfType<CameraHandler>();
            TurnTransition turnTransition = FindObjectOfType<TurnTransition>();
            StartCoroutine(turnTransition.FadeOutHUD());
            cameraHandler.SwitchCamera(currentPlayer.GetComponentInChildren<CinemachineVirtualCamera>());
            StartCoroutine(DelayedHUDFadeIn(turnTransition));
            turnTimer.ResetTimer();
        }

        private void CalculateCurrentPlayerIndex()
        {
            if (currentPlayerIndex < playerWhoHasDied)
            {
                currentPlayerIndex++;
            }

            playerWhoHasDied = playerIndex.Count + 1;

            if (currentPlayerIndex >= currentlyAlivePlayers.Count)
            {
                currentPlayerIndex = 0;
            }
        }

        private IEnumerator DelayedHUDFadeIn(TurnTransition turnTransition)
        {
            yield return new WaitForSeconds(.5f);
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

            List<GameObject> oldCurrentlyAlivePlayers = new List<GameObject>();
            foreach (GameObject player in currentlyAlivePlayers)
            {
                oldCurrentlyAlivePlayers.Add(player);
            }
            currentlyAlivePlayers.Clear();
            foreach (GameObject player in playerIndex)
            {
                if (!player.GetComponent<Health>().isDead)
                {
                    currentlyAlivePlayers.Add(player);
                }
            }
            for(int i = 0; i < oldCurrentlyAlivePlayers.Count; ++i) 
            {
                if (!currentlyAlivePlayers.Contains(oldCurrentlyAlivePlayers[i]))
                {
                    playerWhoHasDied = i;
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

        public void SetupCurrentPlayerWeapons(GameObject currentPlayer)
        {
            if(currentPlayerWeapons.Count > 0)
            {
            currentPlayerWeapons.Clear();
            }
            foreach (Weapon weapon in currentPlayer.GetComponentsInChildren<Weapon>())
            {
                currentPlayerWeapons.Add(weapon.gameObject);
                //Debug.Log(currentPlayer.gameObject.name + weapon.name);
            }
        }
    }
}
