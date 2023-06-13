using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Omega.UI;
using Omega.Actions;
using UnityEngine.InputSystem.UI;

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
        private RoundCompletion roundCompletion;

        private void Awake()
        {
            roundCompletion = FindObjectOfType<RoundCompletion>();
            spawnHandler = GetComponent<PlayerSpawnHandler>();
            playerId = GetComponent<PlayerIdentifier>();
            scoreHandler = GetComponent<ScoreHandler>();
            turnTimer = GetComponent<TurnTimer>();
            turnTransition = FindObjectOfType<TurnTransition>();
            diceCalculator = GetComponent<PhysicalDiceCalculator>();
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

            FindObjectOfType<InputSystemUIInputModule>().enabled = true;
        }

        public void StartNextRound()
        {
            spawnHandler.ResetPlayers();

            StartCoroutine(roundCompletion.FadeOutHUD(roundResults));

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

            EnergyBar energyBar = FindObjectOfType<EnergyBar>();
            energyBar.UpdateSegments();

            FindObjectOfType<Pause>().canPause = true;

            PlayerHealthDisplay playerHealthDisplay = FindObjectOfType<PlayerHealthDisplay>();
            playerHealthDisplay.UpdateHealthInfo();

            ResetEnergyGain();
        }

        public void EndRound()
        {
            ResetEnergyGain();

            spawnHandler.EndRound();

            FindObjectOfType<CameraHandler>().EndRoundCam();

            playerId.roundOver = true;

            gameHUD.alpha = 0;
            gameHUD.interactable = false;
            turnTimer.SetTimeOff();
            scoreHandler.ResetScoreThisRound();

            diceCalculator.ClearDice();

            FindObjectOfType<Pause>().canPause = false;

            FindObjectOfType<SoundtrackMixer>().PlayMenu();
        }



        public void EndGame(GameObject endScreen, bool quit)
        {
            ResetEnergyGain();

            spawnHandler.EndRound();

            if (!quit)
            {
                FindObjectOfType<CameraHandler>().EndRoundCam();
            }

            playerId.roundOver = true;

            if (!quit)
            {
                scoreHandler.DisplayEndGameScores(endScreen.GetComponent<EndScreenCollection>());
            }

            gameHUD.alpha = 0;
            gameHUD.interactable = false;
            turnTimer.SetTimeOff();
            scoreHandler.ResetScoreThisRound();

            currentRound = 0;

            scoreHandler.playerScores.Clear();
            scoreHandler.playerScoresInOrder.Clear();
            diceCalculator.ClearDice();

            FindObjectOfType<SoundtrackMixer>().PlayMenu();

            FindObjectOfType<Pause>().canPause = false;
        }
        private void ResetEnergyGain()
        {
            playerId.amountOfRoundsPassed = 0;
            playerId.amountOfTurnsPassed = 0;
            playerId.energyGainPerTurn = 2;
        }
    }
}