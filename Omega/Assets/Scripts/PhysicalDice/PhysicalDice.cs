using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    public class PhysicalDice : MonoBehaviour
    {
        public Rigidbody diceRigidbody;
        public int diceValue;

        public PhysicalDiceSide[] diceSides;

        [HideInInspector]
        public bool hasLanded;
        [HideInInspector]
        public bool thrown;

        PhysicalDiceCalculator PhysicalDiceCalculator;

        private void Awake()
        {
            PhysicalDiceCalculator = FindObjectOfType<PhysicalDiceCalculator>();
            if (diceRigidbody == null)
            {
                diceRigidbody.GetComponent<Rigidbody>();
                diceRigidbody.useGravity = false;
            }
            PhysicalDiceCalculator.actionDices.Add(gameObject);
        }
        private void Update()
        {
            if(diceRigidbody.IsSleeping() && !hasLanded && thrown)
            {
                hasLanded = true;
                diceRigidbody.useGravity = false;
                diceRigidbody.isKinematic = true;
            }
            else if(diceRigidbody.IsSleeping() && hasLanded && diceValue == 0)
            {
                Debug.Log("Did not land correctly");
            }
        }


        [ContextMenu("RollDice")]
        public void RollDice()
        {
            if(!hasLanded && !thrown)
            {
                thrown = true;
                diceRigidbody.useGravity = true;
                diceRigidbody.AddForce(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10), ForceMode.Impulse);
                diceRigidbody.AddTorque(Random.Range(0, 300), Random.Range(0, 300), Random.Range(0, 300));
            }
            else if(thrown && hasLanded)
            {
                ResetDice();
            }
        }

        public void ResetDice()
        {
            thrown = false;
            hasLanded = false;
            diceRigidbody.useGravity = false;
            diceRigidbody.isKinematic = true;
            Debug.Log("Dice Finished");
            //Destroy(gameObject);
        }

        
    }
}