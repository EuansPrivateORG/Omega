using Cinemachine;
using Omega.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Omega.UI
{
    public class NumbersDisplay : MonoBehaviour
    {
        
        [SerializeField] public TextMeshProUGUI numbersText;
        CanvasGroup canvasGroup;
        float fadeTime = 1f;
        float distance = 2f;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (numbersText == null)
            {
                 numbersText = GetComponent<TextMeshProUGUI>();
            }
        }

        private void Update()
        {
            gameObject.transform.LookAt(Camera.main.transform);
        }

        public void SpawnNumbers(int valueToSpawn)
        {
            
            canvasGroup.alpha = 1;
            numbersText.text = valueToSpawn.ToString();
            StartCoroutine(FadeOut());
            StartCoroutine(MoveUp());

        }

        public IEnumerator FadeIn()
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeTime);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        public IEnumerator FadeOut()
        {
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
                yield return null;
            }
            canvasGroup.alpha = 0f;
            Destroy(gameObject);
        }

        private IEnumerator MoveUp()
        {
            float elapsedTime = 0f;
            Vector3 startingPos = transform.position;
            Vector3 targetPos = startingPos + Vector3.up * distance;

            while (elapsedTime < fadeTime)
            {
                float t = elapsedTime / fadeTime;
                transform.position = Vector3.Lerp(startingPos, targetPos, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPos;
        }
    }
    }
