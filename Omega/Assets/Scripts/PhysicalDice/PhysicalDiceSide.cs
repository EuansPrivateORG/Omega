using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    public class PhysicalDiceSide : MonoBehaviour
    {
        [Tooltip("This needs to be the value on the opposite side of the dice")]
        public int sideValue;

        private bool onGround = false;
        PhysicalDice physicalDice;

        private void Awake()
        {
            physicalDice = GetComponentInParent<PhysicalDice>();
        }

        private void OnTriggerEnter(Collider other)
        {
            GetComponent<AudioSource>().Play();
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.tag == "Terrain")
            {
                onGround = true;
                physicalDice.diceValue = sideValue;       
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Terrain")
            {
                onGround = false;
                physicalDice.diceValue = 0;
            }
        }
    }
}
