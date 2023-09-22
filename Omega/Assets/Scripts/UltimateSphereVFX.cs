using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateSphereVFX : MonoBehaviour
{
    public AnimationCurve scaleCurve; // The animation curve for scaling
    public float targetScale = 5f; // The target scale of the sphere
    public float scalingSpeed = 1f; // The speed at which the sphere scales

    private Vector3 initialScale; // The initial scale of the sphere
    private float currentTime = 0f; // Current time for the animation curve evaluation

    private bool hasEnteredCamera = false; // Flag to track if the sphere has entered the camera's collider

    private void Start()
    {
        initialScale = transform.localScale; // Store the initial scale of the sphere
    }

    private void Update()
    {
        // Increment current time based on scaling speed
        currentTime += scalingSpeed * Time.deltaTime;

        // Evaluate the animation curve at the current time
        float scaleMultiplier = scaleCurve.Evaluate(currentTime);

        // Scale the sphere using the evaluated scale multiplier
        transform.localScale = initialScale * Mathf.Lerp(1f, targetScale, scaleMultiplier);

        // Check if the sphere has entered the camera's collider
        if (!hasEnteredCamera && IsInCameraCollider())
        {
            // Trigger your desired action
            DoSomething();

            hasEnteredCamera = true; // Set the flag to true to prevent repeated triggering
        }

    }

    private bool IsInCameraCollider()
    {
        // Get the main camera's collider (assuming it has one)
        Collider cameraCollider = Camera.main.GetComponent<Collider>();

        if (cameraCollider != null)
        {
            // Check if the sphere's bounds intersect with the camera's collider bounds
            return GetComponent<Renderer>().bounds.Intersects(cameraCollider.bounds);
        }

        return false;
    }

    private void DoSomething()
    {
        WhiteFade whiteFade = FindObjectOfType<WhiteFade>();
        whiteFade.fadeDuration = whiteFade.fadeDuration / 4;
        whiteFade.StartFade(false);
    }
}
