using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningTimer : MonoBehaviour
{
    [SerializeField] public float spinTime;
    [SerializeField] public bool isNegative;
    private bool turningTimer = true;
    private int amountOfSpins = 0;
    private float currentRotation;

    void Update()
    {
        if (turningTimer)
        {
            if (!isNegative)
            {
                currentRotation = 360 + (amountOfSpins * 360);
            }
            else
            {
                currentRotation = -360 + (amountOfSpins * -360);
            }
            LeanTween.rotateZ(gameObject, currentRotation, spinTime);
            turningTimer = false;
        }
        else
        {
            float rotationDifference = Mathf.Abs(gameObject.transform.rotation.eulerAngles.z - currentRotation);
            if (rotationDifference < 0.01f) // Adjust the threshold as needed
            {
                turningTimer = true;
                amountOfSpins++;
                Debug.LogError("here");
            }
        }

        Debug.Log(gameObject.name + " " + gameObject.transform.rotation.eulerAngles.z);
    }
}

