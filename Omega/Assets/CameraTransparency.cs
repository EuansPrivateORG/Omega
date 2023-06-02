using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransparency : MonoBehaviour
{

    public Camera mainCam;
    private void Awake()
    {
        mainCam = Camera.main;
        mainCam.transparencySortMode = TransparencySortMode.Orthographic;
    }
}
