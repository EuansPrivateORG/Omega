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


        public void AddScorer(Base players, int playerNum)
        {
            ScoreValues playerScore = new ScoreValues();
            playerScore.playerFaction = players.factionName;
            playerScore.playerNumReference = playerNum;
            playerScores.Add(playerScore);
        }
    }
}