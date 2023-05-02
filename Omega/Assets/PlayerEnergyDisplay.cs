using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Omega.Status
{
    public class PlayerEnergyDisplay : MonoBehaviour
    {
        private int playerEnergy;
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
            UpdateEnergyText();
        }

        private void UpdateEnergyText()
        {
            if (playerIdentifier.currentPlayer != null)
            {
                playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>().energy;
            }
            DisplayText.text = playerEnergy.ToString();
        }
    }

}
