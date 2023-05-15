using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    [CreateAssetMenu(fileName = "New PlayerAction", menuName = "Create New PlayerAction")]

    public class PlayerAction : ScriptableObject
    {
        [Header("Rolling")]
        public int numOfD4;
        public int numOfD6;

        [Tooltip("What is added to the roll")] public int rollBonus;

        [Header("Price")]
        public int cost;

        public int minDamageFromDice()
        {
            int numToReturn = 0;
            for (int i = 0; i < numOfD4; i++)
            {
                numToReturn++;
            }
            for (int i = 0; i < numOfD6; i++)
            {
                numToReturn++;
            }
            numToReturn += rollBonus;
            return numToReturn;
        }

        public int maxDamageFromDice()
        {
            int numToReturn = 0;
            for (int i = 0; i < numOfD4; i++)
            {
                numToReturn += 4;
            }
            for (int i = 0; i < numOfD6; i++)
            {
                numToReturn += 6;
            }
            numToReturn += rollBonus;
            return numToReturn;
        }
    }
}