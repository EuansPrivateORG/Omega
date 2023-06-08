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
        public AudioClip nextTurnClip;

        public List<GameObject> playerIndex = new List<GameObject>();
        public List<GameObject> turnOrderIndex = new List<GameObject>();
        public List<GameObject> currentlyAlivePlayersInTurn = new List<GameObject>();
        public List<GameObject> currentlyAlivePlayers = new List<GameObject>(); 

        [HideInInspector] public AttackButtonHandler currentAttack;
        [HideInInspector] public HealingButtonHandler currentHeal;
        private CardButtonHandler cardButtonHandler;

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
        private CardTween cardTween;
        private PlayerSpawnHandler playerSpawnHandler;
        private CardHandler cardHandler;

        [HideInInspector] public bool flippedTurnOrder = false;

        [HideInInspector] public bool isSelectingPlayer = false;

        [HideInInspector] public Card speedCard;

        ScoreHandler scoreHandler;


        private void Awake()
        {
            scoreHandler = FindObjectOfType<ScoreHandler>();
            cardButtonHandler = FindObjectOfType<CardButtonHandler>();
            turnTimer = GetComponent<TurnTimer>();
            physicalDiceCalculator = GetComponent<PhysicalDiceCalculator>();
            cardSpawner = GetComponent<CardSpawner>();
            drawCardHandler = FindObjectOfType<DrawCardHandler>();
            cardTween = FindObjectOfType<CardTween>();
            playerSpawnHandler = GetComponent<PlayerSpawnHandler>();
            cardHandler = GetComponent<CardHandler>();
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
            playerIndex = new List<GameObject>();
            playerIndex.AddRange(playerList);
            SetupTurnOrderIndex();
            SetupCurrentlyAlivePlayerIndex();

            currentPlayer = currentlyAlivePlayers[0];
            currentPlayer.GetComponent<Energy>().GainEnergy(energyGainPerTurn);

            UpdatePlayerIcon();

            cardSpawner.ClearActiveCards();

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
                SetupCurrentlyAlivePlayerIndex();

                CalculateCurrentPlayerIndex();

                UpdatePlayerIcon();

                SettingUpNextPlayer();

                CancelHandler handler = FindObjectOfType<CancelHandler>();
                if (handler != null)
                {
                    handler.ResetUI();
                }

                EventSystem.current.SetSelectedGameObject(drawCardHandler.skipButton.gameObject);

                currentAttack = null;
                currentHeal = null;

                cardSpawner.ClearActiveCards();

                drawCardHandler.CheckEnergy();

                cardButtonHandler.CheckCards();

                cardSpawner.SpawnCards(currentPlayer.GetComponent<PlayerCards>().cardsInHand);

                cardTween.RefreshCardList();

                cardHandler.CheckPlayersCards(); //Must be last action apart from other card stuff

                if (speedCard != null)
                {
                    SpeedRound();
                }

                AudioSource audioSource = GetComponent<CardHandler>().cardPlayedSource;
                audioSource.clip = nextTurnClip;
                audioSource.Play();
            }
        }

        public void SpeedRound()
        {
            turnTimer.turnTimeLimit = speedCard.speedRoundTime;
            TimerCollection timerCollection = FindObjectOfType<TimerCollection>();
            timerCollection.gizomo1.GetComponent<RotateUIImage>().rotationSpeed = timerCollection.gizomo1.GetComponent<RotateUIImage>().rotationSpeed * 4;
            timerCollection.gizomo1.GetComponent<RotateUIImage>().rotationSpeed = timerCollection.gizomo2.GetComponent<RotateUIImage>().rotationSpeed * 4;
        }

        private void SettingUpNextPlayer()
        {
            currentPlayer = currentlyAlivePlayersInTurn[currentPlayerIndex];

            Energy currentEnergy = currentPlayer.GetComponent<Energy>();
            if(currentEnergy.energy + energyGainPerTurn <= 16)
            {
                currentEnergy.GainEnergy(energyGainPerTurn);
            }
            else
            {
                currentEnergy.energy = 16;
            }
            PlayerCards currentCards = currentPlayer.GetComponent<PlayerCards>();
            List<Card> cards = new List<Card>();
            cards.AddRange(currentCards.cardsPlayed);
            foreach(Card card in cards)
            {
                if(card.cardType == Card.CardType.eot)
                {
                    currentEnergy.GainEnergy(card.energyPerTurn);
                    currentPlayer.GetComponent<PlayerSetup>().amountOfRoundsEOT--;
                    if(currentPlayer.GetComponent<PlayerSetup>().amountOfRoundsEOT <= 0)
                    {
                        currentCards.cardsPlayed.Remove(card);
                        currentCards.RemovePlayedCards(card.CardWorldPreFab);
                        currentPlayer.GetComponent<BaseVFX>().EnergyVFXStop();
                    }
                }
            }

            CameraHandler cameraHandler = FindObjectOfType<CameraHandler>();
            TurnTransition turnTransition = FindObjectOfType<TurnTransition>();
            StartCoroutine(turnTransition.FadeOutHUD());
            EnergyBar energyBar = FindObjectOfType<EnergyBar>();
            energyBar.UpdateSegments();
            PlayerHealthDisplay playerHealthDisplay = FindObjectOfType<PlayerHealthDisplay>();
            playerHealthDisplay.UpdateHealthInfo();
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

        public void FlipTurnOrder()
        {
            if (flippedTurnOrder)
            {
                flippedTurnOrder = false;
            }
            else flippedTurnOrder = true;

            int currentPlayerIndexInOrder = 0;

            for (int i = 0; i < turnOrderIndex.Count; ++i)
            {
                if (turnOrderIndex[i] == currentPlayer)
                {
                    currentPlayerIndexInOrder = i;
                }
            }

            ReverseTurnOrder(turnOrderIndex, currentPlayerIndexInOrder);
            ReverseTurnOrder(currentlyAlivePlayersInTurn, currentPlayerIndex);
            ReverseScoreOrder(scoreHandler.playerScores, currentPlayerIndexInOrder);

            for (int i = 0; i < currentlyAlivePlayers.Count; i++)
            {
                currentlyAlivePlayers[i].GetComponent<PlayerSetup>().playerID = i + 1;
            }

            for (int i = 0; i < scoreHandler.playerScores.Count; i++)
            {
                scoreHandler.playerScores[i].playerNumReference = i + 1;
            }

            currentPlayerIndex = 0;

            playerSpawnHandler.ResetPlayerIcons();

            UpdatePlayerIcon();
        }

        public static void ReverseTurnOrder(List<GameObject> turnOrder, int currentPlayerNum)
        {
            // Get the current player
            GameObject currentPlayer = turnOrder[currentPlayerNum];

            // Reverse the order of the turnOrder list
            turnOrder.Reverse();

            // Find the new index of the current player
            int newCurrentPlayerIndex = turnOrder.IndexOf(currentPlayer);

            // Move the players before the current player to the end of the list
            List<GameObject> tempPlayers = turnOrder.GetRange(0, newCurrentPlayerIndex);
            turnOrder.RemoveRange(0, newCurrentPlayerIndex);
            turnOrder.AddRange(tempPlayers);
        }

        public static void ReverseScoreOrder(List<ScoreHandler.ScoreValues> scoreOrder, int currentPlayerNum)
        {
            // Get the current player
            ScoreHandler.ScoreValues currentPlayer = scoreOrder[currentPlayerNum];

            // Reverse the order of the turnOrder list
            scoreOrder.Reverse();

            // Find the new index of the current player
            int newCurrentPlayerIndex = scoreOrder.IndexOf(currentPlayer);

            // Move the players before the current player to the end of the list
            List<ScoreHandler.ScoreValues> tempPlayers = scoreOrder.GetRange(0, newCurrentPlayerIndex);
            scoreOrder.RemoveRange(0, newCurrentPlayerIndex);
            scoreOrder.AddRange(tempPlayers);
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
                PlayerIconID iconID = playerSpawner.playerImageList[i].GetComponent<PlayerIconID>();

                float yAxisMovement = iconID.yAxisMovement;

                float oppsiteYAxisMovement = yAxisMovement + iconID.moveDownAmountY;

                if (i  == currentPlayerIndex)
                {
                    LeanTween.moveY(playerSpawner.playerImageList[i], yAxisMovement, .5f);
                }
                else
                {
                    LeanTween.moveY(playerSpawner.playerImageList[i], oppsiteYAxisMovement, .5f);
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
