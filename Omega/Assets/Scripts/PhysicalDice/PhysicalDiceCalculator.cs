using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    public class PhysicalDiceCalculator : MonoBehaviour
    {
        [SerializeField] public int diceTotal;
        public List<GameObject> actionDices = new List<GameObject>();


        [ContextMenu("GetDiceTotal")]
        public void GetDiceTotal()
        {
            if(actionDices.Count > 0)
            {
                foreach (GameObject dice in actionDices)
                {
                    PhysicalDice physicalDice = dice.GetComponent<PhysicalDice>();
                    if(physicalDice.thrown && physicalDice.hasLanded) diceTotal += physicalDice.diceValue;
                }
            }
        }

    }
}
