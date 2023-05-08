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
        [SerializeField] public Button nextRoundButton;
        [SerializeField] public TextMeshProUGUI winningPlayerText;
        private CanvasGroup roundCompletionCanvasGroup;
        private float fadeTime = 1f;
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

                if (roundHandler.numOfRounds == roundHandler.currentRound)
                {
                    StartCoroutine(FadeOutHUD(playerHUDCanvasGroup));
                    StartCoroutine(FadeInHUD(endScreen));
                    roundHandler.EndGame(endScreen.gameObject);
                }
                else
                {
                    scoreHandler.CalculatePlayerPlacement(playerIdentifier.playerIndex.Count, playerIdentifier.currentlyAlivePlayers[0].GetComponent<PlayerSetup>().playerID - 1);
                    winningPlayerText.text = playerIdentifier.currentlyAlivePlayers[0].GetComponent<PlayerSetup>().playerBase.factionName + " Has claimed the outpost";
                    StartCoroutine(FadeOutHUD(playerHUDCanvasGroup));
                    roundCompletionCanvasGroup.interactable = true;
                    EventSystem.current.SetSelectedGameObject(nextRoundButton.gameObject);
                    StartCoroutine(FadeInHUD(roundCompletionCanvasGroup));
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
                group.interactable = true;
                yield return null;
            }

            group.alpha = 1f;
        }

        public void ResetGame()
        {
            print("Resetting Game");
            
        }

    }
}
