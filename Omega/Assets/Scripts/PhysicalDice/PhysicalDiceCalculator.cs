using Omega.Core;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private NumberRoller numberRoller;

        private PlayerIdentifier playerIdentifier;

        [HideInInspector]public bool passedInfo = false;

        public float diceLifeTime = 15f;
        private float diceTimer;

        private void Awake()
        {
            numberRoller = GetComponent<NumberRoller>();
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
                diceTimer += Time.deltaTime;

                if (diceTimer > diceTotal)
                {
                    foreach (GameObject actionDice in actionDices)
                    {
                        PhysicalDice physDice = actionDice.GetComponent<PhysicalDice>();
                        if (physDice.diceValue <= 0)
                        {
                            int ran = Random.Range(0, 4);
                            physDice.diceValue = ran;
                        }
                    }
                }

                foreach (GameObject dice in actionDices)
                {
                    PhysicalDice physicalDice = dice.GetComponent<PhysicalDice>();
                    if (physicalDice.thrown && physicalDice.hasLanded && physicalDice.GetComponent<Rigidbody>().IsSleeping() && physicalDice.diceValue != 0)
                    {
                        diceTotal += physicalDice.diceValue;
                        diceCounter++;
                    }
                
                }
                if(diceCounter == actionDices.Count && !passedInfo)
                {
                    numberRoller.StopRolling(diceTotal,playerIdentifier.isAttacking);
    
                    passedInfo = true;

                    diceTimer = 0;
                }
                diceTotal = 0;
                diceCounter = 0;
            }
        }


        // We should probably move where this function is called to when the projectile is launched as the attack data/ damage is already locked in.
        public void ClearDice()
        {
            StartCoroutine(WaitForDestroy());

        }


        public IEnumerator WaitForDestroy()
        {
            yield return StartCoroutine(DissolveDice());
            foreach (GameObject dice in actionDices)
            {
                Destroy(dice);
            }
            actionDices.Clear();
        }


        private IEnumerator DissolveDice()
        {
            float dissolveDuration = Random.Range(0.7f, 1.5f);
            float elapsedTime = 0f;

            while (elapsedTime < dissolveDuration)
            {
                float t = elapsedTime / dissolveDuration;

                foreach (GameObject dice in actionDices)
                {
                    dice.GetComponent<Renderer>().material.SetFloat("_Dissolve", t);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            foreach (GameObject dice in actionDices)
            {
                dice.GetComponent<Renderer>().material.SetFloat("_Dissolve", 1f);
            }
        }
    }
}
