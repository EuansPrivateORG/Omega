using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Core
{
    public class TurnTimer : MonoBehaviour
    {
        [SerializeField] public float turnTimeLimit = 10f;
        [SerializeField] private float timeLeft;

        private bool timeOn = false;
        PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();
        }


        private void Start()
        {
            timeOn = true;
        }
        private void Update()
        {
            if (timeOn)
            {
                if(timeLeft <= turnTimeLimit)
                {
                    timeLeft += Time.deltaTime;
                }
                else
                {
                    timeOn = false;
                    timeLeft = 0;
                    playerIdentifier.NextPlayer();
                }
            }
        }

        public void ResetTimer()
        {
            timeOn = true;
        }
    }

}