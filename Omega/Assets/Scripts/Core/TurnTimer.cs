using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Omega.Core
{
    public class TurnTimer : MonoBehaviour
    {
        [SerializeField] public float turnTimeLimit = 10f;
        [SerializeField] public Image timeFace;

        private float timeLeft;
        private bool timeOn = false;
        PlayerIdentifier playerIdentifier;
        

        private void Awake()
        {
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
                }
                else
                {
                    timeOn = false;
                    timeLeft = 0;
                    timeFace.fillAmount = 0;
                    playerIdentifier.NextPlayer();
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
    }

}