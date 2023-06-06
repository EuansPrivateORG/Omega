using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Omega.UI
{
    public class Pause : MonoBehaviour
    {
        public GameObject pauseMenu;
        public Button resumeButton;
        public Button quitToMenuButton;

        public Button buttonToResetTo;

        private PlayerInput playerInput;
        private RoundHandler roundhandler;
        private EventSystem eventSystem;

        private GameObject previousSelection;

        private bool canPause = true;
        private void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            roundhandler = FindObjectOfType<RoundHandler>();
            playerInput = FindObjectOfType<PlayerInput>();

        }

        private void Update()
        {
            playerInput.actions["Pause"].performed += Pause_performed;
        }

        private void Pause_performed(InputAction.CallbackContext context)
        {
            
            if(pauseMenu.activeInHierarchy == false && canPause)
            {
                previousSelection = eventSystem.currentSelectedGameObject;
                EnablePauseMenu();
            }
            else if (pauseMenu.activeInHierarchy == true && canPause)
            {
                DisablePauseMenu();
            }
        }

        public void EnablePauseMenu()
        {
            eventSystem.SetSelectedGameObject(resumeButton.gameObject);
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        public void DisablePauseMenu()
        { 
            if(previousSelection != null && previousSelection != quitToMenuButton)
            {
            eventSystem.SetSelectedGameObject(previousSelection);
            }
            else
            {
                eventSystem.SetSelectedGameObject(buttonToResetTo.gameObject);
            }
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            previousSelection = null;
        }


        public void QuitToMenu()
        {
            //quitting behaviour
            Debug.Log("Quit to main menu");
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }
}
