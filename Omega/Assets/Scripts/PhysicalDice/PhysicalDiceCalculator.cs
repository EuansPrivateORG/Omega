using Omega.Core;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

namespace Omega.Actions
{
    public class PhysicalDiceCalculator : MonoBehaviour
    {
        [SerializeField] public int diceTotal;
        public List<GameObject> actionDices = new List<GameObject>();
        private int diceCounter = 0;

        private NumberRoller numberRoller;

        private PlayerIdentifier playerIdentifier;

        [HideInInspector]public bool passedInfo = false;

        private void Awake()
        {
            numberRoller = GetComponent<NumberRoller>();
            playerIdentifier = GetComponent<PlayerIdentifier>();
        }

        private void Update()
        {
            GetDiceTotal(); 
        }

        [ContextMenu("GetDiceTotal")]
        public void GetDiceTotal()
        {
            if(actionDices.Count > 0)
            {
                foreach (GameObject dice in actionDices)
                {
                    PhysicalDice physicalDice = dice.GetComponent<PhysicalDice>();
                    if (physicalDice.thrown && physicalDice.hasLanded && physicalDice.GetComponent<Rigidbody>().IsSleeping() && physicalDice.diceValue != 0)
                    {
                        diceTotal += physicalDice.diceValue;
                        diceCounter++;
                    }
                
                }
                if(diceCounter == actionDices.Count && !passedInfo)
                {
                    numberRoller.StopRolling(diceTotal,playerIdentifier.isAttacking);
    
                    passedInfo = true;
                }
                diceTotal = 0;
                diceCounter = 0;
            }
        }

        public void ClearDice()
        {
            foreach(GameObject dice in actionDices)
            {
                Destroy(dice);
            }
            actionDices.Clear();
        }
    }
}
