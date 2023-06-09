using Omega.Core;
using Omega.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class WhiteFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public float waitBetweenFadeIn = 1f;
    private float originalDuration;

    private Coroutine currentFadeCoroutine;


    private void Start()
    {
        originalDuration = fadeDuration;
    }

    // Call this function to start the fade effect
    public void StartFade(bool startRound)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
            currentFadeCoroutine = StartCoroutine(MidFade());
        }
        else
        {
            currentFadeCoroutine = StartCoroutine(FadeRoutine(startRound));
        }
    }

    private IEnumerator FadeRoutine(bool startRound)
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = Color.clear;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = Color.white;

        if (startRound)
        {
            FindObjectOfType<RoundStart>().continueFade = true;
        }

        // Wait for a brief duration
        yield return new WaitForSeconds(waitBetweenFadeIn);
        fadeDuration = originalDuration;

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = Color.clear;
        fadeImage.gameObject.SetActive(false);

        currentFadeCoroutine = null;
    }

    private IEnumerator MidFade()
    {
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        fadeImage.color = new Color (1,1,1,0.75f);

        // Fade in
        while (elapsedTime < fadeDuration / 4)
        {
            float t = elapsedTime / (fadeDuration / 4);
            fadeImage.color = Color.Lerp(new Color(1, 1, 1, 0.75f), Color.white, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = Color.white;
        // Wait for a brief duration
        yield return new WaitForSeconds(waitBetweenFadeIn);
        fadeDuration = originalDuration;

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = Color.clear;
        fadeImage.gameObject.SetActive(false);

        currentFadeCoroutine = null;
    }
}