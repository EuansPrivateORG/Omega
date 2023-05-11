using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    public class DiceType : MonoBehaviour
    {
        [Header("Dice Prefabs")]
        [SerializeField] public GameObject D4;
        [SerializeField] public GameObject D6;
        [SerializeField] public GameObject D20;
    }
}