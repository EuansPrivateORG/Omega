using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SelectionSound : MonoBehaviour
{
    private GameObject currentlySelectedObject;
    private GameObject previouslySelectedObject;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        GameObject currentSelectedObject = EventSystem.current.currentSelectedGameObject;
        if (currentSelectedObject != null && currentSelectedObject != previouslySelectedObject)
        {
            previouslySelectedObject = currentSelectedObject;
            PlaySelectionSound();
        }
    }

    private void PlaySelectionSound()
    {
        audioSource.Play();
    }
}

