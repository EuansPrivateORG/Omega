using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using TMPro;
using Unity.Mathematics;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Omega.Combat;

namespace Omega.UI
{
    public class HealingButtonHandler : ActionButtonHandler
    {
        public PlayerAction heal;

        private PlayerIdentifier playerIdentifier;
        [SerializeField] public GameObject healingNumbersPrefab;
        [SerializeField] public Gradient colourGradient;

        private ScoreHandler scoreHandler;
        private DiceSpawner diceSpawner;
        private CanvasGroup playerHUD;
        private CancelHandler cancelHandler;
        private GameObject nextAvailablePlayer;
        [HideInInspector] public GameObject playerToHeal;

        [HideInInspector] public bool currentlySelectingPlayer = false;
        private bool healedMax = false;
        private int oldCurrentHealth;
        private NumberRoller numberRoller;

        [HideInInspector] public bool isDoubleRoll = false;

        [HideInInspector] public int rollBonus;

        public bool isHealButton = false;

        PhysicalDiceCalculator physicalDiceCalculator;

        HealingButtonID healingButtonID;

        private void Awake()
        {
            healingButtonID = FindObjectOfType<HealingButtonID>();
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            diceSpawner = FindObjectOfType<DiceSpawner>();
            playerHUD = FindObjectOfType<RoundHandler>().playerHUD;
            cancelHandler = GetComponent<CancelHandler>();
            numberRoller = FindObjectOfType<NumberRoller>();
            physicalDiceCalculator = FindObjectOfType<PhysicalDiceCalculator>();
        }

        private void OnEnable()
        {
            if (!isHealButton)
            {
                Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

                if (playerEnergy.energy < heal.cost)
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;

                    int placeInList = 0;

                    for (int i = 0; i < healingButtonID.heals.Count; i++)
                    {
                        if (healingButtonID.heals[i] == this)
                        {
                            placeInList = i; break;
                        }
                    }
                    if (placeInList != healingButtonID.heals.Count - 1)
                    {
                        Button thisButton = GetComponent<Button>();

                        if (playerEnergy.energy < healingButtonID.heals[placeInList + 1].heal.cost)
                        {
                            Navigation newNav = new Navigation();
                            newNav.mode = Navigation.Mode.Explicit;
                            if (placeInList != 0) newNav.selectOnLeft = healingButtonID.heals[placeInList - 1].GetComponent<Button>();
                            thisButton.navigation = newNav;
                        }
                        else
                        {
                            Navigation newNav = new Navigation();
                            newNav.mode = Navigation.Mode.Explicit;
                            newNav.selectOnRight = healingButtonID.heals[placeInList + 1].GetComponent<Button>();
                            if (placeInList != 0) newNav.selectOnLeft = healingButtonID.heals[placeInList - 1].GetComponent<Button>();
                            thisButton.navigation = newNav;
                        }
                    }
                }
            }
        }

        private void OnDisable()
        {
            List<GameObject> healablePlayers = new List<GameObject>();
            DisableBaseSelection(healablePlayers);
        }

        public void ButtonPressed()
        {
            playerIdentifier.isAttacking = false;

            foreach(GameObject player in playerIdentifier.playerIndex)
            {
                player.GetComponent<PlayerSelectionHandler>().GetCurrentAction(null, this);
            }

            List<GameObject> healablePlayers = new List<GameObject>();

            EnableBaseSelection(healablePlayers);

            EventSystem eventSystem = EventSystem.current;

            eventSystem.SetSelectedGameObject(nextAvailablePlayer);
        }

        private void EnableBaseSelection(List<GameObject> healablePlayers)
        {
            playerIdentifier.isSelectingPlayer = true;
            currentlySelectingPlayer = true;
            playerHUD.alpha = 0.25f;
            cancelHandler.cannotCancel = true;
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                PlayerSelectionHandler playerSelectionHandler = player.GetComponent<PlayerSelectionHandler>();
                playerSelectionHandler.enabled = true;
            }

            bool foundNextPlayer = false;

            for (int i = 0; i < playerIdentifier.turnOrderIndex.Count; i++)
            {
                GameObject playerObject = playerIdentifier.turnOrderIndex[i];

                if (!playerObject.GetComponent<Health>().isDead)
                {
                    playerObject.GetComponent<PlayerSelectionHandler>().enabled = true;
                    playerObject.GetComponent<Selectable>().enabled = true;
                    playerObject.GetComponentInChildren<Outline>().enabled = true;
                    playerObject.GetComponentInChildren<Outline>().OutlineColor = Color.white;
                    healablePlayers.Add(playerObject);

                    if (!foundNextPlayer)
                    {
                        nextAvailablePlayer = playerObject;
                        foundNextPlayer = true;
                    }
                }
            }

            if (!foundNextPlayer)
            {
                nextAvailablePlayer = null;
            }

            foreach (GameObject playerObject in playerIdentifier.currentlyAlivePlayersInTurn)
            {
                BaseSelectionHandlerOverwrite baseSelectionHandlerOverwrite = playerObject.GetComponent<BaseSelectionHandlerOverwrite>();
                baseSelectionHandlerOverwrite.playersToChooseFrom = healablePlayers;
                baseSelectionHandlerOverwrite.InitialNav();
            }
        }

        private void DisableBaseSelection(List<GameObject> healablePlayers)
        {
            playerIdentifier.isSelectingPlayer = false;
            currentlySelectingPlayer = false;
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                PlayerSelectionHandler playerSelectionHandler = player.GetComponent<PlayerSelectionHandler>();

                playerSelectionHandler.enabled = false;

            }
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                if (player != null)
                {
                    player.GetComponent<PlayerSelectionHandler>().enabled = false;
                    player.GetComponent<Selectable>().enabled = false;
                    player.GetComponentInChildren<Outline>().enabled = false;
                    healablePlayers.Add(player);
                }
            }

        }


        public void RollDice(GameObject toHeal)
        {
            int currentRollBonus = heal.rollBonus;

            PlayerCards playerCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            List<Card> cardList = new List<Card>();
            foreach (Card card in playerCards.cardsPlayed)
            {
                cardList.Add(card);
            }

            foreach (Card card in cardList)
            {
                if (card.activationType == Card.ActivationType.onDiceRoll)
                {
                    switch (card.cardType)
                    {
                        case Card.CardType.chaosDice:

                            diceSpawner.ActivateChaosDice();
                            playerCards.cardsPlayed.Remove(card);
                            playerCards.RemovePlayedCards(card.CardWorldPreFab);
                            break;

                        case Card.CardType.rollBonus:

                            currentRollBonus += card.rollBonusValue;
                            playerCards.cardsPlayed.Remove(card);
                            playerCards.RemovePlayedCards(card.CardWorldPreFab);
                            break;

                        case Card.CardType.doubleRoll:

                            isDoubleRoll = true;
                            playerCards.cardsPlayed.Remove(card);
                            playerCards.RemovePlayedCards(card.CardWorldPreFab);
                            break;

                    }
                }
            }

            rollBonus = currentRollBonus;

            numberRoller.rollers.gameObject.SetActive(true);
            numberRoller.StartRolling();

            numberRoller.AddBonusNumbers(rollBonus);

            playerIdentifier.currentHeal = this;
            playerToHeal = toHeal;
            diceSpawner.ActivateDice(heal);

            List<GameObject> healablePlayers = new List<GameObject>();
            DisableBaseSelection(healablePlayers);
        }

        public void PerformHealing(int extraHealth, Card fromCard, GameObject playerToHealth)
        {
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

            playerEnergy.SpendEnergy(heal.cost);
            if (fromCard == null) playerToHealth = playerToHeal;

            Health playerHealth = playerToHealth.GetComponent<Health>();

            HealingSFXCollection sfx = playerToHealth.GetComponentInChildren<HealingSFXCollection>();
            sfx.healingSource.Play();
            PlayerCards playerCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            List<Card> cardList = new List<Card>();
            foreach (Card card in playerCards.cardsPlayed)
            {
                cardList.Add(card);
            }

            foreach (Card card in cardList)
            {
                if(card.cardType == Card.CardType.hot)
                {
                    playerCards.cardsPlayed.Remove(card);
                    playerCards.RemovePlayedCards(card.CardWorldPreFab);
                    playerToHealth.GetComponent<PlayerCards>().cardsPlayedAgainst.Add(card);
                    playerToHealth.GetComponent<PlayerSetup>().amountOfRoundsHOT = card.amountOfRounds;
                    playerToHealth.GetComponent<BaseVFX>().HOTVFXStart();
                }
            }

            if (isDoubleRoll) extraHealth *= 2;

            isDoubleRoll = false;

            extraHealth += rollBonus;

            if (playerHealth.currentHealth + extraHealth >= playerHealth.maxHealth)
            {
                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].pointsHealed += (playerHealth.maxHealth - playerHealth.currentHealth);

                oldCurrentHealth = playerHealth.currentHealth;
                playerHealth.currentHealth = playerHealth.maxHealth;
                healedMax = true;
                Debug.Log(playerHealth.maxHealth - oldCurrentHealth + " Health Healed");
            }
            else
            {
                playerHealth.AddHealth(extraHealth);
                scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].pointsHealed += extraHealth;
                healedMax = false;
                Debug.Log(extraHealth + " Health Healed");
            }

            if (healedMax)
            {
                SpawnHealingNumbers(playerHealth.maxHealth - oldCurrentHealth, playerToHealth, false);
            }
            else
            {
                SpawnHealingNumbers(extraHealth, playerToHealth, false);
            }

            if (!fromCard)
            {
                playerToHealth.GetComponent<BaseVFX>().PerformHealing();
                StartCoroutine(DelayNextTurn());
            }


            playerToHealth.GetComponent<DamageStateCollection>().CheckHealth();


            rollBonus = 0;

            physicalDiceCalculator.ClearDice();
        }

        public void SpawnHealingNumbers(int healthHealed, GameObject playerHeal, bool isCard)
        {
            int minColour = healthHealed - 5;
            int maxColour = healthHealed - 5;

            if (!isCard)
            {
                minColour = heal.minDamageFromDice();
                maxColour = heal.maxDamageFromDice();
            }

            GameObject numbersPrefab = Instantiate(healingNumbersPrefab, playerHeal.transform.position, quaternion.identity);

            numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = GetColorOnGradient(healthHealed, minColour, maxColour, colourGradient);
            NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
            numbersDisplay.SpawnNumbers(healthHealed);
            
        }

        private IEnumerator DelayNextTurn()
        {
            yield return new WaitForSeconds(1f);

            numberRoller.TurnOffNumberRoller();
            playerIdentifier.NextPlayer();

            yield return null;
        }

        public Color GetColorOnGradient(int value, int minValue, int maxValue, Gradient colorGradient)
        {
            float position = (float)(value - minValue) / (maxValue - minValue);
            return colorGradient.Evaluate(position);
        }
    }
}