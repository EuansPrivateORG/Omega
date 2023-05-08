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
            GameObject player = playerIdentifier.currentlyAlivePlayers[playerID];
            int newPlayerID = playerID;

            for(int i = 0; i < playerIdentifier.playerIndex.Count; i++)
            {
                if (playerIdentifier.playerIndex[i] == player)
                {
                    newPlayerID = i;
                }
            }


            if (placement == playerIdentifier.playerIndex.Count)
            {
                playerScores[newPlayerID].placementScore += (placement - 1) + firstPlayerBonus;
            }
            else if (placement == playerIdentifier.playerIndex.Count - 1)
            {
                playerScores[newPlayerID].placementScore += (placement - 1) + secondPlayerBonus;
            }
            else if (placement == playerIdentifier.playerIndex.Count - 2)
            {
                playerScores[newPlayerID].placementScore += (placement - 1) + thirdPlayerBonus;
            }
            else
            {
                playerScores[newPlayerID].placementScore += (placement - 1);
            }
        }
    }
}