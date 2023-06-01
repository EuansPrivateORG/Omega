using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Omega.Core
{
    public class TurnTimer : MonoBehaviour
    {
        [SerializeField] public float turnTimeLimit = 10f;
        [SerializeField] public Image timeFace;
        [SerializeField] public TextMeshProUGUI timerText;
        [SerializeField] public Image timeOutImage;
        [HideInInspector] public float originalTime;

        private bool flashing = false;
        public float toggleDuration = .1f;
        public int repeatCount = 3;

        private float timeLeft;
        private bool timeOn = false;
        PlayerIdentifier playerIdentifier;
        

        private void Awake()
        {
            originalTime = turnTimeLimit;
            playerIdentifier = GetComponent<PlayerIdentifier>();
        }

        private void Update()
        {
            if (timeOn)
            {
                if(timeLeft <= turnTimeLimit)
                {
                    timeLeft += Time.deltaTime;
                    timeFace.fillAmount = Mathf.Clamp01(timeLeft/turnTimeLimit);
                    DisplayTime();
                    if(turnTimeLimit- timeLeft <= 5 && !flashing)
                    {
                       StartCoroutine(flashOnLowTime()); 
                    }
                }
                else
                {
                    timeOn = false;
                    flashing = false;
                    timeLeft = 0;
                    timeFace.fillAmount = 0;
                    playerIdentifier.NextPlayer();
                }
            }
        }

        public void DisplayTime()
        {
            int roundedValue = Mathf.FloorToInt(turnTimeLimit - timeLeft);
            if (roundedValue < 10)
            {
                timerText.text = "00:0" + roundedValue.ToString();
            }

            else if( roundedValue < 0)
            {
                timerText.text = "00:00";
            }
            else
            {
            timerText.text = "00:" + roundedValue.ToString();
            }
        }


        private IEnumerator flashOnLowTime()
        {
            flashing = true;
            int currentRepeat = 0;

            while (currentRepeat < repeatCount)
            {
                // Turn the GameObject off
                timeOutImage.gameObject.SetActive(false);

                // Wait for the specified duration
                yield return new WaitForSeconds(toggleDuration);

                // Turn the GameObject on
                timeOutImage.gameObject.SetActive(true);

                // Wait for the specified duration
                yield return new WaitForSeconds(toggleDuration);

                currentRepeat++;
                if (currentRepeat == repeatCount)
                {
                    timeOutImage.gameObject.SetActive(false);
                }
            }

        
        }

        public void ResetTimer()
        {
            timeOn = true;
            timeLeft = 0;
        }

        public void SetTimeOn()
        {
            timeOn = true;
        }

        public void SetTimeOff()
        {
            timeOn = false;
            timeLeft = 0;
        }
    }

}