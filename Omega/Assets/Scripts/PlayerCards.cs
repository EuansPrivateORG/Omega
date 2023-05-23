using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Omega.Actions
{
    public class PlayerCards : MonoBehaviour
    {
        public List<Card> cardsInHand = new List<Card>();
        public List<Card> cardsPlayed = new List<Card>();
        public List<Card> cardsPlayedAgainst = new List<Card>();

        public List<Transform> cardDeckPositions;

        public List<GameObject> playingCardInDeck;

        public List<Transform> playingCardPositions;

        public List<GameObject> playedCardInWorld;

        public TextMeshProUGUI cardNumText;

        private CardHandler cardHandler;

        private void Awake()
        {
            cardHandler = FindObjectOfType<CardHandler>();
        }

        void Start()
        {

        }

        public void StartingCards()
        {
            cardHandler.DrawCardNoUI(gameObject, 2);
        }

        void Update()
        {

        }
    }
}