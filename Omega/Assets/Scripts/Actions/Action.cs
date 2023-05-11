using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    [CreateAssetMenu(fileName = "New Action", menuName = "Create New Action")]

    public class Action : ScriptableObject
    {
        [Header("Rolling")]
        public int numOfD4;
        public int numOfD6;

        [Tooltip("What is added to the roll")] public int rollBonus;

        [Header("Price")]
        public int cost;
    }
}