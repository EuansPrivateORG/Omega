using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Omega.UI
{
    public class TurnTransition : MonoBehaviour
    {

        public CanvasGroup playerHUDGroup;
        public float fadeTime = .5f;

        public InputSystemUIInputModule inputSystemUIInputModule;
        public PlayerInput playerInput;

        private void Awake()
        {
            playerHUDGroup.alpha = 0f;
        }
        public IEnumerator FadeOutHUD()
        {
            inputSystemUIInputModule.enabled = false;
            playerInput.enabled = false;

            float elapsedTime = 0f;
            if (playerHUDGroup.alpha == 0) yield return null;
            while (elapsedTime < fadeTime * 1.5f)
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

            inputSystemUIInputModule.enabled = true;
            playerInput.enabled = true;

            playerHUDGroup.alpha = 1f;
        }
    }
}
