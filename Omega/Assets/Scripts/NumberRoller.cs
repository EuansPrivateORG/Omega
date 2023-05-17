using Omega.Core;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Omega.Actions
{
    public class NumberRoller : MonoBehaviour
    {
        public RollerCollection rollers;

        public TextMeshProUGUI bonusNumText;

        [Tooltip("The colour the text will change to for the final numbers")]
        public Color rolledNumColours;

        private NumbersCollection hundredsCollection;
        private NumbersCollection tensCollection;
        private NumbersCollection unitsCollection;

        [Tooltip("the rotation speed in degrees per second")]
        public float speed = 10f;

        private bool isRotatingHundreds = false;
        private bool isRotatingTens = false;
        private bool isRotatingUnits = false;

        private bool stoppedRotatingHundreds = false;
        private bool stoppedRotatingTens = false;
        private bool stoppedRotatingUnits = false;

        private bool hasFinsishedHundreds = false;
        private bool hasFinishedTens = false;
        private bool hasFinishedUnits = false;

        [HideInInspector] public int diceTotal;
        [HideInInspector] public int hundredsNumToStop;
        [HideInInspector] public int tensNumToStop;
        [HideInInspector] public int unitsNumToStop;

        private PlayerIdentifier playerIdentifier;

        private Color oldNumberColour;

        [HideInInspector] public bool isAttacking;

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();
        }

        void Update()
        {
            Vector3 camToObject = Vector3.Normalize(transform.position - Camera.main.transform.forward);

            // Rotate each object individually
            if (isRotatingHundreds)
            {
                rollers.hundredsRoller.transform.Rotate(new Vector3(speed * Time.deltaTime, 0, 0));
                if (stoppedRotatingHundreds)
                {
                    var dotProduct = Vector3.Dot(hundredsCollection.numbersList[hundredsNumToStop].transform.forward, camToObject);

                    if (dotProduct <= 0.94 && dotProduct > 0.938)
                    {
                        isRotatingHundreds = false;
                        stoppedRotatingHundreds = false;
                        hasFinsishedHundreds = true;
                        oldNumberColour = hundredsCollection.numbersList[hundredsNumToStop].GetComponentInChildren<TextMeshProUGUI>().color;
                        hundredsCollection.numbersList[hundredsNumToStop].GetComponentInChildren<TextMeshProUGUI>().color = rolledNumColours;
                    }
                }
            }

            if (isRotatingTens)
            {
                rollers.tensRoller.transform.Rotate(new Vector3(speed * Time.deltaTime, 0, 0));
                if (stoppedRotatingTens)
                {
                    var dotProduct = Vector3.Dot(tensCollection.numbersList[tensNumToStop].transform.forward, camToObject);

                    if (dotProduct <= 0.94 && dotProduct > 0.938)
                    {
                        isRotatingTens = false;
                        stoppedRotatingTens = false;
                        hasFinishedTens = true;
                        tensCollection.numbersList[tensNumToStop].GetComponentInChildren<TextMeshProUGUI>().color = rolledNumColours;
                    }
                }
            }

            if (isRotatingUnits)
            {
                rollers.unitsRoller.transform.Rotate(new Vector3(speed * Time.deltaTime, 0, 0));
                if (stoppedRotatingUnits)
                {
                    var dotProduct = Vector3.Dot(unitsCollection.numbersList[unitsNumToStop].transform.forward, camToObject);

                    if (dotProduct <= 0.94 && dotProduct > 0.938)
                    {
                        isRotatingUnits = false;
                        stoppedRotatingUnits = false;
                        hasFinishedUnits = true;
                        unitsCollection.numbersList[unitsNumToStop].GetComponentInChildren<TextMeshProUGUI>().color = rolledNumColours;
                    }
                }
            }

            AllFunctionsFinished();
        }


        public void StopRolling(int _diceTotal, bool isAttack)
        {
            isAttacking = isAttack;

            int hundreds = _diceTotal / 100;
            int tens = (_diceTotal / 10) % 10;
            int units = _diceTotal % 10;

            diceTotal = _diceTotal;

            hundredsNumToStop = hundreds;
            stoppedRotatingHundreds = true;

            tensNumToStop = tens;
            stoppedRotatingTens = true;

            unitsNumToStop = units;
            stoppedRotatingUnits = true;
        }

        public void StartRolling()
        {
            isRotatingHundreds = true;
            hundredsCollection = rollers.hundredsRoller.GetComponent<NumbersCollection>();

            isRotatingTens = true;
            tensCollection = rollers.tensRoller.GetComponent<NumbersCollection>();

            isRotatingUnits = true;
            unitsCollection = rollers.unitsRoller.GetComponent<NumbersCollection>();
        }

        public void AddBonusNumbers(int bonus)
        {
            bonusNumText.transform.parent.LookAt(Camera.main.transform);
            bonusNumText.text = "+" + bonus;
            bonusNumText.color = rolledNumColours;
        }

        public void AllFunctionsFinished()
        {
            if(hasFinishedTens && hasFinishedUnits && hasFinsishedHundreds)
            {
                if (isAttacking)
                {
                    playerIdentifier.currentAttack.PerformAttack(diceTotal);
                }
                else
                {
                    playerIdentifier.currentHeal.PerformHealing(diceTotal);
                }

                hasFinishedTens = false;
                hasFinishedUnits = false;
                hasFinsishedHundreds = false;
            }
        }

        public void TurnOffNumberRoller()
        {
            unitsCollection.numbersList[unitsNumToStop].GetComponentInChildren<TextMeshProUGUI>().color = oldNumberColour;
            tensCollection.numbersList[tensNumToStop].GetComponentInChildren<TextMeshProUGUI>().color = oldNumberColour;
            hundredsCollection.numbersList[hundredsNumToStop].GetComponentInChildren<TextMeshProUGUI>().color = oldNumberColour;

            rollers.gameObject.SetActive(false);
        }
    }
}