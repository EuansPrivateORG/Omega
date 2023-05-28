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
            if(isNegative)
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
        if (gameObject.transform.rotation.z == currentRotation)
        {
            turningTimer = true;
            amountOfSpins++;
        }
    }
}
