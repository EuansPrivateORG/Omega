using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateUIImage : MonoBehaviour
{ 
    public RectTransform imageToRotate;
    public float rotationSpeed = 50f;
    public bool rotateCounterclockwise = false;

    private void Awake()
    {
        imageToRotate = GetComponent<RectTransform>();
    }
    private void Update()
    {
        // Determine the rotation direction based on the rotateCounterclockwise variable
        float direction = rotateCounterclockwise ? -1f : 1f;

        // Rotate the image along the z-axis over time
        imageToRotate.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
    }
}
