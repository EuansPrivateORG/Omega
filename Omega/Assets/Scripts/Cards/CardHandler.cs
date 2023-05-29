using Newtonsoft.Json.Bson;
using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Omega.Actions
{
    public class CardHandler : MonoBehaviour
    {
        public int cardCost = 2;

        public int cardsToStartWith = 2;

        public int maxCards = 5;

        public List<Card> cards;

        public CardProbabilityTracker cardProbabilityTracker;

        private PlayerIdentifier playerIdentifier;

        private DrawCardHandler drawCardHandler;

        public float nextTurnDelayTime;

        private void Awake()
        {
            drawCardHandler = FindObjectOfType<DrawCardHandler>();

            playerIdentifier = GetComponent<PlayerIdentifier>();
        }

        public void DrawCardNoUI(GameObject player, int numOfCards)
        {
            List<Card> cards = new List<Card>();
            PlayerCards playerCards = player.GetComponent<PlayerCards>();

            if(playerCards.cardsInHand.Count > 5)
            {
                Debug.Log("Player has max Cards");
                return;
            }

            for (int i = 0; i < numOfCards; i++)
            {
                bool hasCard = false;
                Card newCard = null;

                newCard = FindCard(player);

                foreach (Card card in cards)
                {
                    if (card == newCard)
                    {
                        i--;
                        hasCard = true;
                    }
                }

                foreach (Card card in playerCards.cardsInHand)
                {
                    if (card == newCard)
                    {
                        i--;
                        hasCard = true;
                    }
                }

                foreach (Card card in playerCards.cardsPlayed)
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
                playerCards.cardsInHand.Add(card);
                playerCards.InstantiatedCardInDeck(card.CardWorldPreFab);
                cardProbabilityTracker.AddCardProb(card);
            }
        }

        public void DrawCard(GameObject player, int numOfCards)
        {
            List<Card> cards = new List<Card>();
            PlayerCards playerCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            if (playerCards.cardsInHand.Count + numOfCards > 5)
            {
                Debug.Log("Player has max Cards");
                return;
            }

            for (int i = 0; i < numOfCards; i++)
            {
                bool hasCard = false;
                Card newCard = null;

                newCard = FindCard(player);

                foreach (Card card in cards)
                {
                    if (card == newCard)
                    {
                        i--;
                        hasCard = true;
                    }
                }

                foreach (Card card in playerCards.cardsInHand)
                {
                    if (card == newCard)
                    {
                        i--;
                        hasCard = true;
                    }
                }

                foreach (Card card in playerCards.cardsPlayed)
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
                playerCards.cardsInHand.Add(card);

                playerCards.InstantiatedCardInDeck(card.CardWorldPreFab);

                cardProbabilityTracker.AddCardProb(card);
            }
        }

        private Card FindCard(GameObject player)
        {
            List<Card> allCards = new List<Card>();
            allCards.AddRange(cards);

            List<float> probabilities = new List<float>();

            float totalWeight = 0;
            foreach (Card card in allCards)
            {
                totalWeight += card.cardWeight; // Assuming each card has a weight property
            }

            float cumulativeProbability = 0;
            foreach (Card card in allCards)
            {
                float cardProbability = card.cardWeight / totalWeight;
                cumulativeProbability += cardProbability;
                probabilities.Add(cumulativeProbability);
            }

            float randomValue = Random.value;

            for (int i = 0; i < probabilities.Count; i++)
            {
                if (randomValue <= probabilities[i])
                {
                    return allCards[i];
                }
            }

            // If no card is selected, return null or handle the case as appropriate
            Debug.LogError("No Card Was Given");
            return null;
        }


        public void CardPlayed(GameObject card)
        {
            Card currentCard = card.GetComponent<CardCollection>().card;
            PlayerCards playersCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();
            PlayerSetup currentPlayersSetup = playerIdentifier.currentPlayer.GetComponent<PlayerSetup>();
            playersCards.cardsInHand.Remove(currentCard);
            playersCards.RemoveCardFromDeck(currentCard.CardWorldPreFab);

            switch (currentCard.activationType)
            {
                case Card.ActivationType.instantActivation:

                    switch (currentCard.cardType)
                    {
                        case Card.CardType.overcharge:

                            if((playerIdentifier.currentPlayer.GetComponent<Energy>().energy + currentCard.energyAmount) <= 16) 
                            { 
                                playerIdentifier.currentPlayer.GetComponent<Energy>().energy += currentCard.energyAmount;
                            }
                            else
                            {
                                playerIdentifier.currentPlayer.GetComponent<Energy>().energy = 16;
                            }
                                
                            FindObjectOfType<EnergyBar>().UpdateSegments();
                            break;

                        case Card.CardType.instantHeal:

                            HealingButtonHandler h = FindObjectOfType<HealingButtonHandler>();
                            h.playerToHeal = playerIdentifier.currentPlayer;
                            h.PerformHealing(currentCard.instantHealAmount, currentCard, playerIdentifier.currentPlayer);
                            break;

                        case Card.CardType.damageReduction:

                            currentPlayersSetup.ActivateDamageReduction(currentCard.damageReductionPreFab);
                            playersCards.cardsPlayed.Add(currentCard);
                            playersCards.InstantiatePlayedCards(currentCard.CardWorldPreFab);
                            break;

                        case Card.CardType.shield:

                            currentPlayersSetup.amountOfRoundsShield = currentCard.amountOfRounds;
                            currentPlayersSetup.ActivateShield(currentCard.shieldPrefab);
                            playersCards.cardsPlayed.Add(currentCard);
                            playersCards.InstantiatePlayedCards(currentCard.CardWorldPreFab);
                            break;

                        case Card.CardType.flipTurn:

                            playerIdentifier.FlipTurnOrder();
                            break;

                        case Card.CardType.eot:

                            currentPlayersSetup.amountOfRoundsEOT = currentCard.amountOfRounds;
                            playersCards.cardsPlayed.Add(currentCard);
                            playersCards.InstantiatePlayedCards(currentCard.CardWorldPreFab);
                            break;
                    }
                    break;

                default:

                    if (currentCard.effectOverTime)
                    {
                        switch (currentCard.cardType)
                        {
                            case Card.CardType.shield:

                                currentPlayersSetup.amountOfRoundsShield = currentCard.amountOfRounds;
                                break;
                        }
                    }

                    playersCards.cardsPlayed.Add(currentCard);
                    playersCards.InstantiatePlayedCards(currentCard.CardWorldPreFab);
                    break;
            }

            if(playersCards.cardsInHand.Count < 5)
            {
                Button drawCardButton = drawCardHandler.GetComponent<Button>();
                drawCardButton.interactable = true;

                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = drawCardButton;
                newNav.selectOnLeft = drawCardHandler.attackButton;
                drawCardHandler.healingButton.navigation = newNav;

                Navigation newNav2 = new Navigation();
                newNav2.mode = Navigation.Mode.Explicit;
                newNav2.selectOnRight = drawCardHandler.healingButton;
                newNav2.selectOnLeft = drawCardButton;
                drawCardHandler.skipButton.navigation = newNav2;
            }
        }

        public void CheckPlayersCards()
        {
            PlayerCards playersCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            PlayerSetup playerSetup = playerIdentifier.currentPlayer.GetComponent<PlayerSetup>();

            List<Card> cardList = new List<Card>();
            foreach(Card card in playersCards.cardsPlayed)
            {
                cardList.Add(card);
            }

            foreach(Card card in cardList)
            {
                if (card.effectOverTime)
                {

                    if(card.cardType == Card.CardType.shield)
                    {
                        playerSetup.amountOfRoundsShield--;

                        if (playerSetup.amountOfRoundsShield <= 0)
                        {
                            playerSetup.DeActivateShield();
                            playersCards.cardsPlayed.Remove(card);
                            playersCards.RemovePlayedCards(card.CardWorldPreFab);
                        }
                    }
                }
            }

            List<Card> _cardList = new List<Card>();
            foreach (Card card in playersCards.cardsPlayedAgainst)
            {
                _cardList.Add(card);
            }

            foreach (Card card in _cardList)
            {
                if (card.effectOverTime)
                {
                    if(card.cardType == Card.CardType.hot)
                    {
                        HealingButtonHandler heal = FindObjectOfType<HealingButtonHandler>();

                        heal.PerformHealing(card.healingPerTurn, card, playerIdentifier.currentPlayer);
                        StartCoroutine(DelayNums(null, card, heal));

                        playerSetup.amountOfRoundsHOT--;

                        if (playerSetup.amountOfRoundsHOT <= 0)
                        {
                            playersCards.cardsPlayedAgainst.Remove(card);
                        }
                    }

                    if(card.cardType == Card.CardType.dot)
                    {
                        AttackButtonHandler attack = FindObjectOfType<AttackButtonHandler>();

                        playerIdentifier.currentPlayer.GetComponent<Health>().currentHealth -= card.damagePerTurn;
                        StartCoroutine(DelayNums(attack, card, null));

                        playerSetup.amountOfRoundsDOT--;

                        if (playerSetup.amountOfRoundsDOT <= 0)
                        {
                            playersCards.cardsPlayedAgainst.Remove(card);
                        }
                    }

                    if (card.cardType == Card.CardType.stun)
                    {

                        playerSetup.amountOfRoundsStun--;

                        if (playerSetup.amountOfRoundsStun <= 0)
                        {
                            playersCards.cardsPlayedAgainst.Remove(card);
                            StartCoroutine(DelayNextTurn());
                        }
                    }


                }

            }
        }

        public IEnumerator DelayNextTurn()
        {
            yield return new WaitForSeconds(nextTurnDelayTime);
            playerIdentifier.NextPlayer();
        }

        private IEnumerator DelayNums(AttackButtonHandler atk, Card card, HealingButtonHandler heal)
        {
            yield return new WaitForSeconds(nextTurnDelayTime * 0.8f);

            if(atk != null)
            {
                atk.SpawnDamageNumbers(playerIdentifier.currentPlayer, card.damagePerTurn - 5, card.damagePerTurn + 5, true, card.damagePerTurn);
            }
            else
            {
                heal.SpawnHealingNumbers(card.healingPerTurn, playerIdentifier.currentPlayer, true);
            }
        }
    }
}