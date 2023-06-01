using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Omega.Actions
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Create New Card")]

    public class Card : ScriptableObject
    {
        [Header("Pre Fab")]
        public GameObject CardUIPreFab;
        public GameObject CardWorldPreFab;

        [Header("Card Info")]
        public string CardName;

        public enum CardCategory
        {
            Attack,
            Heal,
            Utility
        }

        public CardCategory cardCategory;

        [Tooltip("The higher the card weight the higher the probability of that card being given")]
        public float cardWeight;

        public enum ActivationType
        {
            onAttack,
            onHeal,
            onDiceRoll,
            instantActivation
        }

        public ActivationType activationType;

        public enum CardType
        {
            aoe,
            dot,
            stun,
            shield,
            hot,
            overcharge,
            doubleRoll,
            chaosDice,
            rollBonus,
            flipTurn,
            instantHeal,
            damageReduction,
            eot,
            lifeSteal
        }

        public CardType cardType;

        // Fields for specific card types
        [HideInInspector] public int amountOfRounds; // for over time cards

        [HideInInspector] public int damagePerTurn; //  for dot cards
        [HideInInspector] public GameObject shieldPrefab; // for shield cards
        [HideInInspector] public int healingPerTurn; // for hot cards
        [HideInInspector] public int energyAmount; // for overcharge cards
        [HideInInspector] public int rollBonusValue; // for rollBonus cards
        [HideInInspector] public int instantHealAmount; // for instantHeal cards
        [HideInInspector] public float damageReductionPercentage; //For reduced damage
        [HideInInspector] public GameObject damageReductionPreFab;
        [HideInInspector] public int energyPerTurn; //For Energy over time
        [HideInInspector] public float lifeStealPercentage; //For Energy over time


        public bool effectOverTime;

        public bool hasEffectWhenAttacked = false;

#if UNITY_EDITOR
        [CustomEditor(typeof(Card))]
        public class CardEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                Card card = (Card)target;

                if(card.effectOverTime == true)
                {
                    card.amountOfRounds = EditorGUILayout.IntField("Amount of Rounds", card.amountOfRounds);
                }

                // Display fields based on card type
                switch (card.cardType)
                {
                    case CardType.dot:
                        card.damagePerTurn = EditorGUILayout.IntField("Damage Per Turn", card.damagePerTurn);
                        break;
                    case CardType.shield:
                        card.shieldPrefab = (GameObject)EditorGUILayout.ObjectField("Shield Prefab", card.shieldPrefab, typeof(GameObject), false);
                        break;
                    case CardType.hot:
                        card.healingPerTurn = EditorGUILayout.IntField("Healing Per Turn", card.healingPerTurn);
                        break;
                    case CardType.overcharge:
                        card.energyAmount = EditorGUILayout.IntField("Energy Amount", card.energyAmount);
                        break;
                    case CardType.rollBonus:
                        card.rollBonusValue = EditorGUILayout.IntField("Bonus Roll Value", card.rollBonusValue);
                        break;
                    case CardType.instantHeal:
                        card.instantHealAmount = EditorGUILayout.IntField("Amount to Instant Heal", card.instantHealAmount);
                        break;
                    case CardType.damageReduction:
                        card.damageReductionPercentage = EditorGUILayout.FloatField("Percentage of Reduction", card.damageReductionPercentage);
                        card.damageReductionPreFab = (GameObject)EditorGUILayout.ObjectField("Damage PreFab", card.damageReductionPreFab, typeof(GameObject), false);
                        break;
                    case CardType.eot:
                        card.energyPerTurn = EditorGUILayout.IntField("Amount of energy per turn", card.energyPerTurn);
                        break;
                    case CardType.lifeSteal:
                        card.lifeStealPercentage = EditorGUILayout.FloatField("Percentage of health gained", card.lifeStealPercentage);
                        break;
                }
            }
        }
#endif
    }
}