using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Omega.UI;

namespace Omega.Core
{
    public class RoundHandler : MonoBehaviour
    {
        public int numOfRounds = 5;
        public int currentRound = 0;

        public CanvasGroup roundResults;
        public CanvasGroup gameHUD;

        public List<Base> players;

        private PlayerSpawnHandler spawnHandler;
        private PlayerIdentifier playerId;
        public Button healButton;
        private ScoreHandler scoreHandler;
        TurnTimer turnTimer;

        private void Awake()
        {
            spawnHandler = FindObjectOfType<PlayerSpawnHandler>();
            playerId = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            turnTimer = FindObjectOfType<TurnTimer>();
        }

        public void StartFirstRound(List<Base> playersToSpawn)
        {
            players = playersToSpawn;
            spawnHandler.StartFirstRound(players);
            playerId.roundOver = false;
            turnTimer.SetTimeOn();
        }

        public void StartNextRound()
        {

            currentRound++;
            spawnHandler.StartNextRound(players);
            Debug.Log("Here 3");

            roundResults.alpha = 0;
            roundResults.interactable = false;

            gameHUD.alpha = 1;
            gameHUD.interactable = true;
            EventSystem.current.SetSelectedGameObject(healButton.gameObject);

            playerId.roundOver = false;
            turnTimer.SetTimeOn();
        }

        public void EndRound()
        {
            spawnHandler.ResetPlayers();
            playerId.roundOver = true;

            gameHUD.alpha = 0;
            gameHUD.interactable = false;
            turnTimer.SetTimeOff();
        }

        public void EndGame(GameObject endScreen)
        {
            spawnHandler.ResetPlayers();
            playerId.roundOver = true;

            scoreHandler.DisplayEndGameScores(endScreen.GetComponent<EndScreenCollection>());

            gameHUD.alpha = 0;
            gameHUD.interactable = false;
            turnTimer.SetTimeOff();
        }

        public void ResetGame()
        {

        }
    }
}