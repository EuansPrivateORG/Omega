using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Core;
using Omega.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoSelect : MonoBehaviour
{
    PlayerIdentifier playerIdentifier;
    EventSystem currentEvent;

    GameObject currentButton = null;

    private void Awake()
    {
        playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        currentEvent = EventSystem.current;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Debug.Log(currentButton);
            currentEvent.SetSelectedGameObject(currentButton);
        }
        else
        {
            currentButton = currentEvent.currentSelectedGameObject;
        }
    }
}
