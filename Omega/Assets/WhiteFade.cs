using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private Coroutine currentFadeCoroutine;

    // Call this function to start the fade effect
    public void StartFade()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
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

        // Wait for a brief duration
        yield return new WaitForSeconds(1f);

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