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

        public float minRange = 100f;
        public float maxRange = 300f;

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
            }
            else if(diceRigidbody.IsSleeping() && hasLanded && diceValue == 0)
            {
                diceRigidbody.AddForce(100f, 0f, 100f, ForceMode.Impulse);
                Debug.Log("Did not land correctly");
                hasLanded = false;
            }
            if(diceRigidbody.IsSleeping() && !hasLanded && thrown && diceValue != 0)
            {
                diceRigidbody.useGravity = false;
                diceRigidbody.isKinematic = true;
            }
        }


        [ContextMenu("RollDice")]
        public void RollDice(Transform target)
        {
            if (!hasLanded && !thrown)
            {
                thrown = true;
                diceRigidbody.useGravity = true;

                // Calculate the direction towards the target
                Vector3 direction = (target.position - transform.position).normalized;

                // Calculate the force magnitude based on the distance from the target
                float distance = Vector3.Distance(transform.position, target.position);
                float forceMagnitude = Mathf.Clamp(distance, minRange, maxRange);

                // Apply the force towards the target
                diceRigidbody.AddForce(direction * forceMagnitude, ForceMode.Impulse);

                // Add random torque to make the dice spin
                diceRigidbody.AddTorque(Random.Range(0, 300), Random.Range(0, 300), Random.Range(0, 300));
            }
            else if (thrown && hasLanded)
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
