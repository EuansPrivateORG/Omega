using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.InputSystem.DefaultInputActions;

namespace Omega.Core
{
    public class ScoreHandler : MonoBehaviour
    {
        public int firstPlayerBonus;

        public int secondPlayerBonus;

        public int thirdPlayerBonus;

        private PlayerIdentifier playerIdentifier;

        [System.Serializable]
        public class ScoreValues
        {
            public  string playerFaction = "";
            public  int placementScore = 0;
            public  int playersKilled = 0;
            public  int damageDealt = 0;
            public  int playerNumReference = 0;
        }

        public List<ScoreValues> playerScores = new List<ScoreValues>();

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();
        }

        public void AddScorer(Base players, int playerNum)
        {
            ScoreValues playerScore = new ScoreValues();
            playerScore.playerFaction = players.factionName;
            playerScore.playerNumReference = playerNum;
            playerScores.Add(playerScore);
        }

        public void CalculatePlayerPlacement(int placement, int playerID)
        {
            if(placement == playerIdentifier.playerIndex.Count - 1)
            {
                Debug.Log("Second Place");
                playerScores[playerID - 1].placementScore = (placement - 1) + secondPlayerBonus;
            }
            else if (placement == playerIdentifier.playerIndex.Count - 2)
            {
                Debug.Log("Third Place");
                playerScores[playerID - 1].placementScore = (placement - 1) + thirdPlayerBonus;
            }
            else
            {
                playerScores[playerID - 1].placementScore = (placement - 1);
            }
        }
    }
}