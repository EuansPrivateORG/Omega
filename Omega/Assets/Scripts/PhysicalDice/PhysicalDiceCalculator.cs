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

        private PlayerIdentifier playerIdentifier = null;

        [HideInInspector]public bool passedInfo = false;

        private void Awake()
        {
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
                    if (physicalDice.thrown && physicalDice.hasLanded && physicalDice.GetComponent<Rigidbody>().IsSleeping())
                    {
                        diceTotal += physicalDice.diceValue;
                        diceCounter++;
                    }
                
                }
                if(diceCounter == actionDices.Count && !passedInfo)
                {
                    Debug.Log("here");
                    playerIdentifier.currentAttack.PerformAttack(diceTotal);
                    passedInfo = true;
                }
                diceTotal = 0;
                diceCounter = 0;
            }
        }

    }
}
