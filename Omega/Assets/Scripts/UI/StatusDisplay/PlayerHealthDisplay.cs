using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Omega.Core;
using Omega.Status;
using UnityEngine.UI;

namespace Omega.UI
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        private float playerHealth;
        private float maxHealth;
        public TextMeshProUGUI DisplayText;
        public Image FactionIcon = null;
        public Slider healthSlider;


        PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            if (DisplayText == null)
            {
                DisplayText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }



        public void UpdateHealthInfo()
        {
            if (playerIdentifier.currentPlayer != null)
            {
                Base playerBase = playerIdentifier.currentPlayer.GetComponent<PlayerSetup>().playerBase;
                Image currentPlayerIcon = playerBase.turnOrderVarientIcon.GetComponent<PlayerIconID>().playerIcon.GetComponent<Image>();
                FactionIcon.sprite = currentPlayerIcon.sprite;
                if(playerBase.factionName == "Typherion Dominion")
                {
                    FactionIcon.color = new Color(212f/255f,34f/255f,21f/255f,1f);
                }
                else
                {
                FactionIcon.color = currentPlayerIcon.color;
                }
                playerHealth = playerIdentifier.currentPlayer.GetComponent<Health>().currentHealth;
                maxHealth = playerIdentifier.currentPlayer.GetComponent<Health>().maxHealth;
            }
            healthSlider.value = playerHealth/ maxHealth;
            Debug.Log("Clamp: " + playerHealth/ maxHealth);
            DisplayText.text = playerHealth.ToString();
            Debug.Log(playerIdentifier.currentPlayer.gameObject.name + " : " + playerHealth);
            Debug.Log("Slidervalue: " + healthSlider.value);
        }
    }

}