using Newtonsoft.Json.Bson;
using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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

        public void CardPlayed(GameObject card)
        {
            Card currentCard = card.GetComponent<CardCollection>().card;
            PlayerCards playersCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();
            playersCards.cardsInHand.Remove(currentCard);

            switch (currentCard.activationType)
            {
                case Card.ActivationType.instantActivation:
                    switch (currentCard.cardType)
                    {
                        case Card.CardType.overcharge:

                            playerIdentifier.currentPlayer.GetComponent<Energy>().energy += currentCard.energyAmount;
                            break;

                        case Card.CardType.instantHeal:

                            HealingButtonHandler h = FindObjectOfType<HealingButtonHandler>();
                            h.playerToHeal = playerIdentifier.currentPlayer;
                            h.PerformHealing(currentCard.instantHealAmount, true);
                            break;

                        case Card.CardType.damageReduction:

                            playerIdentifier.currentPlayer.GetComponent<PlayerSetup>().ActivateDamageReduction(currentCard.damageReductionPreFab);
                            playersCards.cardsPlayed.Add(currentCard);
                            break;

                        case Card.CardType.shield:

                            playerIdentifier.currentPlayer.GetComponent<PlayerSetup>().ActivateShield(currentCard.shieldPrefab);
                            playersCards.cardsPlayed.Add(currentCard);
                            break;

                        case Card.CardType.flipTurn:

                            playerIdentifier.FlipTurnOrder();
                            break;
                    }
                    break;
                default:
                    playersCards.cardsPlayed.Add(currentCard);
                    break;
            }

        }

        public void CheckPlayersCards()
        {
            PlayerCards playersCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            List<Card> cardList = new List<Card>();
            foreach(Card card in playersCards.cardsPlayed)
            {
                cardList.Add(card);
            }

            foreach(Card card in cardList)
            {
                if (card.effectOverTime)
                {
                    card.amountOfRounds--;

                    if(card.cardType == Card.CardType.shield && card.amountOfRounds <= 0)
                    {
                        PlayerSetup playerSetup = playersCards.gameObject.GetComponent<PlayerSetup>();

                        playerSetup.DeActivateShield();
                        playersCards.cardsPlayed.Remove(card);
                    }
                }
            }
        }
    }
}