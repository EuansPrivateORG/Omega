using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Omega.Core;

namespace Omega.Status
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        private int playerHealth;
        public TextMeshProUGUI DisplayText;

        PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            if (DisplayText == null)
            {
                DisplayText = GetComponentInChildren<TextMeshProUGUI>();
            }
        }


        private void Update()
        {
            UpdateHealthText();
        }

        private void UpdateHealthText()
        {
            if (playerIdentifier.currentPlayer != null)
            {
                playerHealth = playerIdentifier.currentPlayer.GetComponent<Health>().health;
            }
            DisplayText.text = playerHealth.ToString();
        }
    }

}