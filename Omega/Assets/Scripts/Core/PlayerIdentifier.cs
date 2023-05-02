using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Core
{
    public class PlayerIdentifier : MonoBehaviour
    {
        public List<GameObject> playerIndex = new List<GameObject>();
        public GameObject currentPlayer = null;
        private int currentPlayerIndex = 0;

        public void SetIndex(List<GameObject> playerList)
        {
            playerIndex = playerList;
            currentPlayer = playerIndex[0];

        }

        public void NextPlayer()
        {
            currentPlayerIndex++;
            currentPlayer = playerIndex[currentPlayerIndex];
        }
    }
}
