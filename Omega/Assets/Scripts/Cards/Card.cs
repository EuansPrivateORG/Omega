using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


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

        public enum CardOdds
        {
            Low,
            Medium,
            High
        }

        public CardOdds cardOdds;

        public enum ActivationType
        {
            onAttack,
            onHeal,
            onDiceRoll,
            instantActivation,
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
            flipTurn
        }

        public CardType cardType;

        // Fields for specific card types
        [HideInInspector] public int amountOfRounds; // for over time cards

        [HideInInspector] public int damagePerTurn; //  for dot cards
        [HideInInspector] public GameObject shieldPrefab; // for shield cards
        [HideInInspector] public int healingPerTurn; // for hot cards
        [HideInInspector] public int energyAmount; // for overcharge cards
        [HideInInspector] public int rollBonusValue; // for rollBonus cards

        public bool effectOverTime;

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
                }
            }
        }
#endif
    }
}