using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.UI
{
    public class MenuSwitcher : MonoBehaviour
    {
        [SerializeField] public CanvasGroup group;
        [SerializeField] public float fadeTime = .8f;


        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
        }

        public void LerpIn()
        {
            LeanTween.alphaCanvas(group, 1, fadeTime);
            group.interactable = true;
        }
        public void LerpOut()
        {
            LeanTween.alphaCanvas(group, 0, fadeTime);
            group.interactable = false;
        }
    }
}
