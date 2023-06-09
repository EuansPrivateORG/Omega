using Newtonsoft.Json.Bson;
using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Omega.Actions
{
    public class CardHandler : MonoBehaviour
    {
        public AudioClip attackClip;

        public AudioClip utilityClip;

        public AudioClip healClip;

        public AudioClip takeDamageSound;

        public AudioClip stunSound;

        public AudioSource cardPlayedSource;

        public AudioSource cardPlayedSecondary;

        public int cardCost = 2;

        public int cardsToStartWith = 2;

        public int maxCards = 5;

        public List<Card> cards;

        public CardProbabilityTracker cardProbabilityTracker;

        private PlayerIdentifier playerIdentifier;

        private DrawCardHandler drawCardHandler;

        private CardSpawner cardSpawner;

        public float nextTurnDelayTime;

        bool stunPlayed = false;

        bool disableInput = false;

        InputSystemUIInputModule inputSystemUIInputModule;
        PlayerInput playerInput;

        private void Awake()
        {
            inputSystemUIInputModule = FindObjectOfType<InputSystemUIInputModule>();

            playerInput = FindObjectOfType<PlayerInput>();

            drawCardHandler = FindObjectOfType<DrawCardHandler>();

            playerIdentifier = GetComponent<PlayerIdentifier>();

            cardSpawner = FindObjectOfType<CardSpawner>();
        }

        private void Update()
        {
            if (disableInput)
            {
                inputSystemUIInputModule.enabled = false;
                playerInput.enabled = false;
            }
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

                if (playerIdentifier.currentlyAlivePlayers.Count <= 2)
                {
                    if (newCard.cardType == Card.CardType.flipTurn)
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

        public void DrawCard(GameObject player, int numOfCards, bool endTurn, bool fromKill)
        {
            List<Card> cards = new List<Card>();
            PlayerCards playerCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            if (playerCards.cardsInHand.Count + numOfCards > 5)
            {
                Debug.Log("Player has max Cards");
                if (endTurn)
                {
                    FindObjectOfType<CardHandler>().StartCoroutine(DelayNextTurn(false));
                }
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

                if(playerIdentifier.currentlyAlivePlayers.Count <= 2)
                {
                    if(newCard.cardType == Card.CardType.flipTurn)
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

                cardSpawner.AddCard(card, endTurn, fromKill);
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

            switch (currentCard.cardCategory)
            {
                case Card.CardCategory.Attack:
                    cardPlayedSource.clip = attackClip;
                    cardPlayedSource.Play();
                    break;

                case Card.CardCategory.Utility:
                    cardPlayedSource.clip = utilityClip;
                    cardPlayedSource.Play();
                    break;

                case Card.CardCategory.Heal:
                    cardPlayedSource.clip = healClip;
                    cardPlayedSource.Play();
                    break;
            }


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
                            playerIdentifier.currentPlayer.GetComponent<BaseVFX>().StartOverchargeVFX();   
                            FindObjectOfType<EnergyBar>().UpdateSegments();
                            break;

                        case Card.CardType.instantHeal:

                            HealingButtonHandler h = FindObjectOfType<HealingButtonHandler>();
                            PlayerHealthDisplay pH = FindObjectOfType<PlayerHealthDisplay>();
                            h.playerToHeal = playerIdentifier.currentPlayer;
                            h.PerformHealing(currentCard.instantHealAmount, currentCard, playerIdentifier.currentPlayer);
                            pH.UpdateHealthInfo();
                            playerIdentifier.currentPlayer.GetComponent<BaseVFX>().PerformHealing();
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
                            playerIdentifier.currentPlayer.GetComponent<BaseVFX>().StartEnergyVFX();
                            break;

                        case Card.CardType.speedRound:

                            playersCards.cardsPlayed.Add(currentCard);
                            playersCards.InstantiatePlayedCards(currentCard.CardWorldPreFab);
                            playerIdentifier.speedCard = currentCard;
                            break;

                        case Card.CardType.disruptor:

                            playersCards.cardsPlayed.Add(currentCard);
                            playersCards.InstantiatePlayedCards(currentCard.CardWorldPreFab);
                            playerIdentifier.currentPlayer.GetComponent<BaseVFX>().StartDisruptorVFX(); 
                            break;
                    }
                    break;

                default:

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

                if(card.cardType == Card.CardType.speedRound)
                {
                    playersCards.cardsPlayed.Remove(card);
                    playersCards.RemovePlayedCards(card.CardWorldPreFab);
                    playerIdentifier.speedCard = null;
                    TurnTimer turnTimer = FindObjectOfType<TurnTimer>();
                    turnTimer.turnTimeLimit = turnTimer.originalTime;
                    TimerCollection timerCollection = FindObjectOfType<TimerCollection>();
                    timerCollection.gizomo1.GetComponent<RotateUIImage>().rotationSpeed = timerCollection.gizomo1.GetComponent<RotateUIImage>().rotationSpeed / 4;
                    timerCollection.gizomo1.GetComponent<RotateUIImage>().rotationSpeed = timerCollection.gizomo2.GetComponent<RotateUIImage>().rotationSpeed / 4;
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
                        playerIdentifier.currentPlayer.GetComponent<BaseVFX>().HOTVFXStart();
                        StartCoroutine(DelayNums(null, card, heal));

                        playerIdentifier.currentPlayer.GetComponent<DamageStateCollection>().CheckHealth();

                        playerSetup.amountOfRoundsHOT--;

                        if (playerSetup.amountOfRoundsHOT <= 0)
                        {
                            playerIdentifier.currentPlayer.GetComponent<BaseVFX>().HOTVFXStop();
                            playersCards.cardsPlayedAgainst.Remove(card);
                        }

                        PlayerHealthDisplay playerHealthDisplay = FindObjectOfType<PlayerHealthDisplay>();
                        playerHealthDisplay.UpdateHealthInfo();
                    }

                    if(card.cardType == Card.CardType.dot)
                    {
                        AttackButtonHandler attack = FindObjectOfType<AttackButtonHandler>();

                        playerIdentifier.currentPlayer.GetComponent<Health>().TakeDamage(card.damagePerTurn);
                        StartCoroutine(DelayNums(attack, card, null));
                        playerIdentifier.currentPlayer.GetComponent<DamageStateCollection>().CheckHealth();

                        playerSetup.amountOfRoundsDOT--;

                        if (playerSetup.amountOfRoundsDOT <= 0)
                        {
                            playerIdentifier.currentPlayer.GetComponent<BaseVFX>().DamageVFXStop();
                            playersCards.cardsPlayedAgainst.Remove(card);
                        }
                        if (playerIdentifier.currentPlayer.GetComponent<Health>().currentHealth <= 0)
                        {
                            playerIdentifier.NextPlayer();
                        }

                        PlayerHealthDisplay playerHealthDisplay = FindObjectOfType<PlayerHealthDisplay>();
                        playerHealthDisplay.UpdateHealthInfo();
                    }

                    if (card.cardType == Card.CardType.stun)
                    {
                        playerSetup.amountOfRoundsStun--;
                        StartCoroutine(DelayNextTurn(true));

                        stunPlayed = true;

                        if (playerSetup.amountOfRoundsStun <= 0)
                        {
                            playersCards.cardsPlayedAgainst.Remove(card);
                            playerIdentifier.currentPlayer.GetComponent<BaseVFX>().StunVFXStop();
                        }
                    }


                }

            }
        }

        public IEnumerator DelayNextTurn(bool _disableInput)
        {
            float waitTime = nextTurnDelayTime;
            if (_disableInput)
            {
                disableInput = true;
            }
            yield return new WaitForSeconds(waitTime);

            disableInput = false;

            if (stunPlayed)
            {
                cardPlayedSecondary.clip = stunSound;
                cardPlayedSecondary.Play();
                stunPlayed = false;
            }
            playerIdentifier.NextPlayer();
        }

        private IEnumerator DelayNums(AttackButtonHandler atk, Card card, HealingButtonHandler heal)
        {
            yield return new WaitForSeconds(nextTurnDelayTime * 0.8f);

            if(atk != null)
            {
                atk.SpawnDamageNumbers(playerIdentifier.currentPlayer, card.damagePerTurn - 5, card.damagePerTurn + 5, true, card.damagePerTurn);
                cardPlayedSource.clip = takeDamageSound;
                cardPlayedSource.Play();
            }
            else
            {
                heal.SpawnHealingNumbers(card.healingPerTurn, playerIdentifier.currentPlayer, true);
            }
        }
    }
}