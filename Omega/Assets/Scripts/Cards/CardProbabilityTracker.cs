using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Omega.Actions
{
    [CreateAssetMenu(fileName = "New Card Prob", menuName = "Create New Card Prob")]

    public class CardProbabilityTracker : ScriptableObject
    {
        //This is for our use to see the statistics of how many of each type of card players have gotten

        [Header("Low Chance Cards")]
        public int totalShieldSpawned;
        public int totalStunSpawned;
        public int totalDoubleSpawned;
        [Header("Medium Chance Cards")]
        public int totalAOESpawned;
        public int totalInstantSpawned;
        public int totalDOTSpawned;
        public int totalHOTSpawned;
        public int totalFlipSpawned;
        [Header("High Chance Cards")]
        public int totalDamageReductionSpawned;
        public int totalBonusSpawned;
        public int totalOverchargeSpawned;
        public int totalChaosSpawned;

        public void AddCardProb(Card card)
        {
            switch (card.cardType)
            {
                case Card.CardType.overcharge:

                    totalOverchargeSpawned++;
                    break;

                case Card.CardType.instantHeal:

                    totalInstantSpawned++;
                    break;

                case Card.CardType.damageReduction:

                    totalDamageReductionSpawned++;
                    break;

                case Card.CardType.shield:

                    totalShieldSpawned++;
                    break;
                case Card.CardType.flipTurn:

                    totalFlipSpawned++;
                    break;

                case Card.CardType.aoe:

                    totalAOESpawned++;
                    break;
                case Card.CardType.chaosDice:

                    totalChaosSpawned++;
                    break;
                case Card.CardType.dot:

                    totalDOTSpawned++;
                    break;
                case Card.CardType.hot:

                    totalHOTSpawned++;
                    break;
                case Card.CardType.doubleRoll:

                    totalDoubleSpawned++;
                    break;
                case Card.CardType.rollBonus:

                    totalBonusSpawned++;
                    break;
                case Card.CardType.stun:

                    totalStunSpawned++;
                    break;
            }
        }
    }

}