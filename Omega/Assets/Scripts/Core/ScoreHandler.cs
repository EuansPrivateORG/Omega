using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.InputSystem.DefaultInputActions;
using Omega.UI;

namespace Omega.Core
{
    public class ScoreHandler : MonoBehaviour
    {
        [HideInInspector] public List<GameObject> leaderboardPlayers = new List<GameObject>(); 

        public Transform leaderboardSpawnPos;
        public GameObject leaderboardPreFab;

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
            public int pointsHealed = 0;
            public  int playerNumReference = 0;
            public int pointsGainedThisRound = 0;
        }

        public List<ScoreValues> playerScores = new List<ScoreValues>();
        public List<ScoreValues> playerScoresInOrder = new List<ScoreValues>();

        private RoundHandler roundHandler;

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();
            roundHandler = GetComponent<RoundHandler>();
        }

        public void AddScorer(Base players, int playerNum)
        {
            ScoreValues playerScore = new ScoreValues();
            playerScore.playerFaction = players.factionName;
            playerScore.playerNumReference = playerNum;
            playerScores.Add(playerScore);
            playerScoresInOrder.Add(playerScore);
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
                playerScores[newPlayerID].pointsGainedThisRound += (placement - 1) + firstPlayerBonus;
            }
            else if (placement == playerIdentifier.playerIndex.Count - 1)
            {
                playerScores[newPlayerID].placementScore += (placement - 1) + secondPlayerBonus;
                playerScores[newPlayerID].pointsGainedThisRound += (placement - 1) + secondPlayerBonus;
            }
            else if (placement == playerIdentifier.playerIndex.Count - 2)
            {
                playerScores[newPlayerID].placementScore += (placement - 1) + thirdPlayerBonus;
                playerScores[newPlayerID].pointsGainedThisRound += (placement - 1) + thirdPlayerBonus;
            }
            else
            {
                playerScores[newPlayerID].placementScore += (placement - 1);
                playerScores[newPlayerID].pointsGainedThisRound += (placement - 1);
            }
        }

        public void ReOrderPlacementList()
        {
            playerScoresInOrder.Sort((a, b) =>
            {
                // Sort by placement score in descending order
                int placementComparison = b.placementScore.CompareTo(a.placementScore);
                if (placementComparison != 0)
                {
                    return placementComparison;
                }

                // If placement score is the same, sort by players killed in descending order
                int playersKilledComparison = b.playersKilled.CompareTo(a.playersKilled);
                if (playersKilledComparison != 0)
                {
                    return playersKilledComparison;
                }

                // If players killed are the same, sort by damage dealt in descending order
                int damageDealtComparison = b.damageDealt.CompareTo(a.damageDealt);
                if (damageDealtComparison != 0)
                {
                    return damageDealtComparison;
                }

                // If all else is equal, leave the order as-is
                return 0;
            });
        }

        public void DisplayEndGameScores(EndScreenCollection endScreen)
        {
            ReOrderPlacementList();

            foreach (Base player in roundHandler.players)
            {

                if (player.factionName == playerScoresInOrder[0].playerFaction)
                {
                    PlayerIconID iconID = player.turnOrderVarientIcon.GetComponent<PlayerIconID>();
                    endScreen.player1Icon.sprite = iconID.playerIcon.GetComponent<Image>().sprite;
                    endScreen.player1Icon.color = iconID.iconBackground.GetComponent<Image>().color;
                    endScreen.player1factionBackground.color = iconID.iconBackground.GetComponent<Image>().color;
                }

                else if (player.factionName == playerScoresInOrder[1].playerFaction)
                {
                    PlayerIconID iconID = player.turnOrderVarientIcon.GetComponent<PlayerIconID>();
                    endScreen.player2Icon.sprite = iconID.playerIcon.GetComponent<Image>().sprite;
                    endScreen.player2Icon.color = iconID.iconBackground.GetComponent<Image>().color;
                    endScreen.player2factionBackground.color = iconID.iconBackground.GetComponent<Image>().color;
                }

                else if (player.factionName == playerScoresInOrder[2].playerFaction)
                {
                    PlayerIconID iconID = player.turnOrderVarientIcon.GetComponent<PlayerIconID>();
                    endScreen.player3Icon.sprite = iconID.playerIcon.GetComponent<Image>().sprite;
                    endScreen.player3Icon.color = iconID.iconBackground.GetComponent<Image>().color;
                    endScreen.player3factionBackground.color = iconID.iconBackground.GetComponent<Image>().color;
                }
            }

            endScreen.player1FactionName.text = playerScoresInOrder[0].playerFaction;
            endScreen.totalScorePlayer1.text = playerScoresInOrder[0].placementScore.ToString();
            endScreen.killAmountPlayer1.text = playerScoresInOrder[0].playersKilled.ToString();
            endScreen.damageAmountPlayer1.text = playerScoresInOrder[0].damageDealt.ToString();
            endScreen.healAmountPlayer1.text = playerScoresInOrder[0].pointsHealed.ToString();

            endScreen.player2FactionName.text = playerScoresInOrder[1].playerFaction;
            endScreen.totalScorePlayer2.text = playerScoresInOrder[1].placementScore.ToString();
            endScreen.killAmountPlayer2.text = playerScoresInOrder[1].playersKilled.ToString();
            endScreen.damageAmountPlayer2.text = playerScoresInOrder[1].damageDealt.ToString();
            endScreen.healAmountPlayer2.text = playerScoresInOrder[1].pointsHealed.ToString();

            endScreen.player3FactionName.text = playerScoresInOrder[2].playerFaction;
            endScreen.totalScorePlayer3.text = playerScoresInOrder[2].placementScore.ToString();
            endScreen.killAmountPlayer3.text = playerScoresInOrder[2].playersKilled.ToString();
            endScreen.damageAmountPlayer3.text = playerScoresInOrder[2].damageDealt.ToString();
            endScreen.healAmountPlayer3.text = playerScoresInOrder[2].pointsHealed.ToString();
        }

        public void DisplayLeadboardScores()
        {
            ReOrderPlacementList();

            for (int i = 0; i < playerScoresInOrder.Count; i++)
            {
                Base currentPlayer = null;
                GameObject instantiated = Instantiate(leaderboardPreFab, leaderboardSpawnPos, leaderboardSpawnPos.transform);

                LeaderboardCollection leadboardCollection = instantiated.GetComponent<LeaderboardCollection>();

                leadboardCollection.leaderboardPosition.text = (i + 1).ToString();
                foreach(Base player in roundHandler.players)
                {
                    if(player.factionName == playerScoresInOrder[i].playerFaction)
                    {
                        currentPlayer = player;
                    }
                }

                leadboardCollection.factionIcon.GetComponent<Image>().sprite = currentPlayer.turnOrderVarientIcon.GetComponent<PlayerIconID>().playerIcon.GetComponent<Image>().sprite;
                leadboardCollection.factionIcon.GetComponent<Image>().color = currentPlayer.turnOrderVarientIcon.GetComponent<PlayerIconID>().iconBackground.GetComponent<Image>().color;

                leadboardCollection.factionName.text = playerScoresInOrder[i].playerFaction;

                leadboardCollection.pointsGained.text = "+" + playerScoresInOrder[i].pointsGainedThisRound.ToString();

                leadboardCollection.totalPoints.text = playerScoresInOrder[i].placementScore.ToString();

                leaderboardPlayers.Add(instantiated);
            }
        }

        public void ResetScoreThisRound()
        {
            foreach(ScoreValues score in playerScores)
            {
                score.pointsGainedThisRound = 0;
            }
        }
    }
}