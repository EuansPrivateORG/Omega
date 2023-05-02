using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Core
{
    public class PlayerIdentifier : MonoBehaviour
    {
        [HideInInspector]
        public List<GameObject> playerIndex = new List<GameObject>();
        public GameObject currentPlayer = null;
        private int currentPlayerIndex = 0;
        TurnTimer turnTimer;

        private void Awake()
        {
            turnTimer = GetComponent<TurnTimer>();
        }

        public void SetIndex(List<GameObject> playerList)
        {
            playerIndex = playerList;
            currentPlayer = playerIndex[0];

        }

        public void NextPlayer()
        {
            currentPlayerIndex++;
            if (currentPlayerIndex >= playerIndex.Count)
            {
                currentPlayerIndex = 0;
            }
            currentPlayer = playerIndex[currentPlayerIndex];
            turnTimer.ResetTimer();
        }
    }
}
