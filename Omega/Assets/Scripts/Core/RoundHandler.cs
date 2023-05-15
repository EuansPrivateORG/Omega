using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Omega.UI;
using Omega.Actions;

namespace Omega.Core
{
    public class RoundHandler : MonoBehaviour
    {
        public int numOfRounds = 5;
        public int currentRound = 0;

        public CanvasGroup roundResults;
        public CanvasGroup gameHUD;
        public CanvasGroup playerHUD;

        public List<Base> players;

        private PlayerSpawnHandler spawnHandler;
        private PlayerIdentifier playerId;
        public Button healButton;
        private ScoreHandler scoreHandler;
        TurnTimer turnTimer;
        TurnTransition turnTransition;
        private PhysicalDiceCalculator diceCalculator;

        private void Awake()
        {
            spawnHandler = FindObjectOfType<PlayerSpawnHandler>();
            playerId = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            turnTimer = FindObjectOfType<TurnTimer>();
            turnTransition = FindObjectOfType<TurnTransition>();
            diceCalculator = FindObjectOfType<PhysicalDiceCalculator>();
        }

        public void StartFirstRound(List<Base> playersToSpawn)
        {
            players = playersToSpawn;
            spawnHandler.StartFirstRound(players);
            playerId.roundOver = false;
            turnTimer.SetTimeOn();
            foreach(GameObject player in scoreHandler.leaderboardPlayers)
            {
                Destroy(player);
            }
            scoreHandler.leaderboardPlayers.Clear();
        }

        public void StartNextRound()
        {
            roundResults.alpha = 0;
            roundResults.interactable = false;

            gameHUD.alpha = 1;
            gameHUD.interactable = true;
            StartCoroutine(turnTransition.FadeInHUD());
            currentRound++;
            spawnHandler.StartNextRound(players);
            EventSystem.current.SetSelectedGameObject(healButton.gameObject);

            playerId.roundOver = false;
            turnTimer.SetTimeOn();

            foreach (GameObject player in scoreHandler.leaderboardPlayers)
            {
                Destroy(player);
            }
            scoreHandler.leaderboardPlayers.Clear();
        }

        public void EndRound()
        {
            spawnHandler.ResetPlayers();
            playerId.roundOver = true;

            gameHUD.alpha = 0;
            gameHUD.interactable = false;
            turnTimer.SetTimeOff();
            scoreHandler.ResetScoreThisRound();

            diceCalculator.ClearDice();
        }

        public void EndGame(GameObject endScreen)
        {
            spawnHandler.ResetPlayers();
            playerId.roundOver = true;

            scoreHandler.DisplayEndGameScores(endScreen.GetComponent<EndScreenCollection>());

            gameHUD.alpha = 0;
            gameHUD.interactable = false;
            turnTimer.SetTimeOff();
            scoreHandler.ResetScoreThisRound();

            currentRound = 0;

            scoreHandler.playerScores.Clear();
            scoreHandler.playerScoresInOrder.Clear();
            diceCalculator.ClearDice();
        }
    }
}