using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Omega.Actions
{
    public class PhysicalDiceCalculator : MonoBehaviour
    {
        [SerializeField] public int diceTotal;
        public List<GameObject> actionDices = new List<GameObject>();
        private int diceCounter = 0;

        public TextMeshProUGUI diceDisplayValue;

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
                    if (physicalDice.thrown && physicalDice.hasLanded)
                    {
                        diceTotal += physicalDice.diceValue;
                        diceCounter++;
                    }
                
                }
                if(diceCounter == actionDices.Count)
                {
                    //Call functions once we know final value
                    diceDisplayValue.text = "Dice Total: " + diceTotal.ToString();
                }
                diceCounter = 0;
            }
        }

    }
}
