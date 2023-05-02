using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    [CreateAssetMenu(fileName = "New Dice", menuName = "Create New Dice")]

    public class Dice : ScriptableObject
    {
        [Header("Rolling")]
        public int minimumRoll;
        public int maximumRoll;
        [Tooltip("What is added to the roll")] public int rollBonus;

        [Header("Price")]
        public int cost;


        public int roll()
        {
            int ran = Random.Range(minimumRoll, maximumRoll);

            return ran + rollBonus;
        }
    }
}