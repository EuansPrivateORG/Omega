using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Omega.UI
{
    public class RoundCompletion : MonoBehaviour
    {
        [SerializeField] public CanvasGroup playerHUDCanvasGroup;
        [SerializeField] public Button nextRoundButton;
        private CanvasGroup roundCompletionCanvasGroup;
        private float fadeTime = 1f;

        EventSystem eventSystem;
        PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            roundCompletionCanvasGroup = GetComponent<CanvasGroup>();
            roundCompletionCanvasGroup.interactable = false;
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }

        private void Update()
        {
            if (playerIdentifier.currentlyAlivePlayers.Count == 1)
            {
                StartCoroutine(FadeOutHUD(playerHUDCanvasGroup));
                roundCompletionCanvasGroup.interactable = true;
                EventSystem.current.SetSelectedGameObject(nextRoundButton.gameObject);
                StartCoroutine(FadeInHUD(roundCompletionCanvasGroup));
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
