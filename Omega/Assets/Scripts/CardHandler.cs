using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Omega.Actions
{
    public class CardHandler : MonoBehaviour
    {
        public int cardCost = 2;

        public int cardsToStartWith = 2;

        public int maxCards = 5;

        public List<Card> cards;

        private List<Card> lowChanceCards = new List<Card>();

        private List<Card> mediumChanceCards = new List<Card>();

        private List<Card> highChanceCards = new List<Card>();

        private CardSpawner cardSpawner;

        private PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            cardSpawner = GetComponent<CardSpawner>();

            playerIdentifier = GetComponent<PlayerIdentifier>();

            foreach(Card card in cards)
            {
                if(card.cardOdds == Card.CardOdds.Low)
                {
                    lowChanceCards.Add(card);
                }
                if (card.cardOdds == Card.CardOdds.Medium)
                {
                    mediumChanceCards.Add(card);
                }
                if (card.cardOdds == Card.CardOdds.High)
                {
                    highChanceCards.Add(card);
                }
            }
        }

        public void DrawCardNoUI(GameObject player, int numOfCards)
        {
            List<Card> cards = new List<Card>();
            PlayerCards playerCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            for (int i = 0; i < numOfCards; i++)
            {
                bool hasCard = false;
                Card newCard = null;

                newCard = FindCard(player);
                foreach (Card card in playerCards.cardsInHand)
                {
                    if (card == newCard)
                    {
                        i--;
                        hasCard = true;
                    }
                }

                if (!hasCard)
                {
                    cards.Add(newCard);
                }
            }

            foreach (Card card in cards)
            {
                player.GetComponent<PlayerCards>().cardsInHand.Add(card);
            }
        }

        public void DrawCard(GameObject player, int numOfCards)
        {
            List<Card> cards = new List<Card>();
            PlayerCards playerCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            for (int i = 0; i < numOfCards; i++)
            {
                bool hasCard = false;
                Card newCard = null;

                newCard = FindCard(player);
                foreach (Card card in playerCards.cardsInHand)
                {
                    if (card == newCard)
                    {
                        i--;
                        Debug.Log("Has Card");
                        hasCard = true;
                    }
                }

                if(!hasCard)
                {
                    cards.Add(newCard);
                }

            }
            foreach (Card card in cards)
            {
                player.GetComponent<PlayerCards>().cardsInHand.Add(card);
                cardSpawner.AddCard(card);
            }
        }

        private Card FindCard(GameObject player)
        {
            int ran = Random.Range(0, 100);

            if (ran > 83.33)
            {
                int rand = Random.Range(0, lowChanceCards.Count);
                return lowChanceCards[rand];
            }
            else if (ran < 83.33 && ran > 50)
            {
                int rand = Random.Range(0, mediumChanceCards.Count);
                return mediumChanceCards[rand];
            }
            else
            {
                int rand = Random.Range(0, highChanceCards.Count);
                return highChanceCards[rand];
            }
        }
    }
}