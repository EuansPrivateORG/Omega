using Omega.Actions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Omega.UI
{
    public class ActionCostDisplay : MonoBehaviour
    {
        public TextMeshProUGUI DisplayText;
        AttackButtonHandler attackButtonHandler = null;
        HealingButtonHandler healingButtonHandler = null;
        Dice dice;

        private void Awake()
        {
            if (DisplayText == null)
            {
                DisplayText = GetComponentInChildren<TextMeshProUGUI>();
            }
            attackButtonHandler = GetComponent<AttackButtonHandler>();
            healingButtonHandler = GetComponent<HealingButtonHandler>();

            if(attackButtonHandler == null)
            {
                dice = healingButtonHandler.dice;
            }
            if (healingButtonHandler == null)
            {
                dice = GetComponent<AttackButtonHandler>().dice;
            }

            
        }

        private void Start()
        {
            DisplayText.text = dice.cost.ToString();
        }
    }
}
