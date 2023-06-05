using Omega.Core;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        [Tooltip("the rotation speed in degrees per second")]
        public float speed = 10f;

        public float hundredsDelay = 0.1f;
        public float tensDelay = 0.15f;
        public float unitsDelay = 0.05f;

        private float hundredsTimer = 0;
        private float tensTimer = 0;
        private float unitsTimer = 0;

        TextMeshProUGUI hundredsText;
        TextMeshProUGUI tensText;
        TextMeshProUGUI unitsText;

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

        private int hundredsInt = 0;
        private int tensInt = 0;
        private int unitsInt = 0;

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();
        }

        void Update()
        {
            // Rotate each object individually
            if (isRotatingHundreds)
            {
                hundredsTimer += Time.deltaTime;

                if(hundredsTimer > hundredsDelay)
                {
                    hundredsText.text = hundredsInt.ToString();

                    hundredsInt++;

                    if (hundredsInt > 9)
                    {
                        hundredsInt = 0;
                    }

                    hundredsTimer = 0;
                }

                if (stoppedRotatingHundreds)
                {
                    if(hundredsInt - 1 == hundredsNumToStop)
                    {
                        isRotatingHundreds = false;
                        stoppedRotatingHundreds = false;
                        hasFinsishedHundreds = true;
                        oldNumberColour = hundredsText.GetComponent<TextMeshProUGUI>().color;
                        hundredsText.GetComponent<TextMeshProUGUI>().color = rolledNumColours;
                        hundredsInt = 0;
                    }
                }
            }

            if (isRotatingTens)
            {
                tensTimer += Time.deltaTime;

                if (tensTimer > tensDelay)
                {
                    tensText.text = tensInt.ToString();

                    tensInt++;

                    if (tensInt > 9)
                    {
                        tensInt = 0;
                    }

                    tensTimer = 0;
                }

                if (stoppedRotatingTens)
                {
                    if (tensInt - 1 == tensNumToStop)
                    {
                        isRotatingTens = false;
                        stoppedRotatingTens = false;
                        hasFinishedTens = true;
                        oldNumberColour = tensText.GetComponent<TextMeshProUGUI>().color;
                        tensText.GetComponent<TextMeshProUGUI>().color = rolledNumColours;
                        tensInt = 0;
                    }
                }
            }

            if (isRotatingUnits)
            {
                unitsTimer += Time.deltaTime;

                if (unitsTimer > unitsDelay)
                {
                    unitsText.text = unitsInt.ToString();

                    unitsInt++;

                    if (unitsInt > 9)
                    {
                        unitsInt = 0;
                    }

                    unitsTimer = 0;
                }

                if (stoppedRotatingUnits)
                {
                    if (unitsInt == unitsNumToStop)
                    {
                        isRotatingUnits = false;
                        stoppedRotatingUnits = false;
                        hasFinishedUnits = true;
                        oldNumberColour = unitsText.GetComponent<TextMeshProUGUI>().color;
                        unitsText.GetComponent<TextMeshProUGUI>().color = rolledNumColours;
                        unitsInt = 0;
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
            hundredsText = rollers.hundredsRoller.GetComponent<TextMeshProUGUI>();

            isRotatingTens = true;
            tensText = rollers.tensRoller.GetComponent<TextMeshProUGUI>();

            isRotatingUnits = true;
            unitsText = rollers.unitsRoller.GetComponent<TextMeshProUGUI>();
        }

        public void AddBonusNumbers(int bonus)
        {
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
                    playerIdentifier.currentHeal.PerformHealing(diceTotal, null, null);
                }

                hasFinishedTens = false;
                hasFinishedUnits = false;
                hasFinsishedHundreds = false;
            }
        }

        public void TurnOffNumberRoller()
        {
            rollers.unitsRoller.GetComponent<TextMeshProUGUI>().color = oldNumberColour;
            rollers.tensRoller.GetComponent<TextMeshProUGUI>().color = oldNumberColour;
            rollers.hundredsRoller.GetComponent<TextMeshProUGUI>().color = oldNumberColour;

            rollers.gameObject.SetActive(false);
        }
    }
}