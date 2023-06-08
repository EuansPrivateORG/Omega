using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Omega.UI
{
    public class RoundCompletion : MonoBehaviour
    {
        [SerializeField] public CanvasGroup playerHUDCanvasGroup;
        [SerializeField] public CanvasGroup endScreen;
        [SerializeField] public CanvasGroup startScreen;
        [SerializeField] public Button nextRoundButton;
        [SerializeField] public TextMeshProUGUI winningPlayerText;
        private CanvasGroup roundCompletionCanvasGroup;
        private float fadeTime = 0.25f;
        private bool hasEndedRound = false;

        EventSystem eventSystem;
        PlayerIdentifier playerIdentifier;
        ScoreHandler scoreHandler;
        RoundHandler roundHandler;

        private void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            roundCompletionCanvasGroup = GetComponent<CanvasGroup>();
            roundCompletionCanvasGroup.interactable = false;
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            roundHandler = FindObjectOfType<RoundHandler>();
        }

        private void Update()
        {
            if (playerIdentifier.currentlyAlivePlayers.Count == 1 && !hasEndedRound)
            {
                hasEndedRound = true;
                if (roundHandler.numOfRounds == roundHandler.currentRound + 1)
                {
                    StartCoroutine(FadeInHUD(endScreen));
                    StartCoroutine(FadeOutHUD(playerHUDCanvasGroup));
                    roundHandler.EndGame(endScreen.gameObject, false);
                    EventSystem.current.SetSelectedGameObject(endScreen.GetComponent<EndScreenCollection>().resetButton.gameObject);
                }
                else
                {
                    StartCoroutine(FadeInHUD(roundCompletionCanvasGroup));
                    scoreHandler.CalculatePlayerPlacement(playerIdentifier.turnOrderIndex.Count, playerIdentifier.turnOrderIndex.IndexOf(playerIdentifier.currentlyAlivePlayers[0]));
                    winningPlayerText.text = playerIdentifier.currentlyAlivePlayers[0].GetComponent<PlayerSetup>().playerBase.factionName + " Has claimed the outpost";
                    StartCoroutine(FadeOutHUD(playerHUDCanvasGroup));
                    EventSystem.current.SetSelectedGameObject(nextRoundButton.gameObject);
                    scoreHandler.DisplayLeadboardScores();
                    roundHandler.EndRound();
                }
                hasEndedRound = false;
            }
        }


        public IEnumerator FadeOutHUD(CanvasGroup group)
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                group.alpha = alpha;
                group.interactable = false;
                yield return null;
            }

            if (group == roundCompletionCanvasGroup)
            {
                scoreHandler.leaderboardPlayers.Clear();
            }

            group.alpha = 0f;
        }

        public IEnumerator FadeInHUD(CanvasGroup group)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
                group.alpha = alpha;
                yield return null;
            }

            group.interactable = true;
            group.alpha = 1f;
        }

        public void ResetGame(bool quit)
        {
            FindObjectOfType<PlayerSpawnHandler>().ResetPlayers();

            if (!quit)
            {
                StartCoroutine(FadeOutHUD(endScreen));
            }
            StartCoroutine(FadeInHUD(startScreen));
            EventSystem.current.SetSelectedGameObject(startScreen.GetComponent<RoundStart>().startRoundButton.gameObject);
            FindObjectOfType<CameraHandler>().MainMenuCam();
        }

    }
}
