using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.UI
{
    public class TurnTransition : MonoBehaviour
    {

        public CanvasGroup playerHUDGroup;
        public float fadeTime = .5f;

        private void Awake()
        {
            playerHUDGroup.alpha = 0f;
        }
        public IEnumerator FadeOutHUD()
        {
            float elapsedTime = 0f;
            if (playerHUDGroup.alpha == 0) yield return null;
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                playerHUDGroup.alpha = alpha;
                playerHUDGroup.interactable = false;
                yield return null;
            }

            playerHUDGroup.alpha = 0f;
        }

        public IEnumerator FadeInHUD()
        {

            float elapsedTime = 0f;
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
                playerHUDGroup.alpha = alpha;
                playerHUDGroup.interactable = true;
                yield return null;
            }

            playerHUDGroup.alpha = 1f;
        }
    }
}
