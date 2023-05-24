using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Mathematics;
using TMPro;
using Omega.Combat;


namespace Omega.UI
{
    public class AttackButtonHandler : ActionButtonHandler
    {

        public Weapon.weaponClass weaponClass;
        public PlayerAction attack;

        private PlayerIdentifier playerIdentifier;

        [HideInInspector]
        public int currentDamage;

        private GameObject nextAvailablePlayer;
        [SerializeField] public GameObject damageNumbersPrefab;
        [SerializeField] public Gradient colourGradient;

        private ScoreHandler scoreHandler;
        private DiceSpawner diceSpawner;
        private GameObject playerToDamage;
        public List<GameObject> attackablePlayers = new List<GameObject>();

        public CanvasGroup playerHUD = null;

        [HideInInspector] public int rollBonus;

        private CancelHandler cancelHandler;

        [HideInInspector] public bool currentlySelectingPlayer = false;

        private NumberRoller numberRoller;

        [HideInInspector] public bool isDoubleRoll = false;

        PhysicalDiceCalculator physicalDiceCalculator;

        private GameObject playerLeft = null;
        private GameObject playerRight = null;

        public bool isAttackButton = false;

        [HideInInspector] public List<GameObject> currentlyAttackablePlayers;

        private void Awake()
        {
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
            if (!isAttackButton)
            {
                Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();

                if (playerEnergy.energy < attack.cost)
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;
                }
            }
        }
        private void OnDisable()
        {
            if (!isAttackButton)
            {
                List<GameObject> attackablePlayers = new List<GameObject>();
                DisableBaseSelection(attackablePlayers);
            }
        }

        public void ButtonPressed()
        {
            playerIdentifier.isAttacking = true;

            playerIdentifier.currentAttack = this;

            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                player.GetComponent<PlayerSelectionHandler>().GetCurrentAction(this, null);
            }

            List<GameObject> attackablePlayers = new List<GameObject>();

            EnableBaseSelection(attackablePlayers);

            EventSystem eventSystem = EventSystem.current;

            eventSystem.SetSelectedGameObject(nextAvailablePlayer);

        }

        private void EnableBaseSelection(List<GameObject> attackablePlayers)
        {
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

                Card shieldCard = null;

                foreach(Card card in playerObject.GetComponent<PlayerCards>().cardsPlayed)
                {
                    if(card.cardType == Card.CardType.shield)
                    {
                        shieldCard = card;
                    }
                }

                if(shieldCard != null)
                {
                    continue;
                }

                if (playerObject != playerIdentifier.currentPlayer && !playerObject.GetComponent<Health>().isDead)
                {
                    playerObject.GetComponent <PlayerSelectionHandler>().enabled = true;
                    playerObject.GetComponent<Selectable>().enabled = true;
                    playerObject.GetComponentInChildren<Outline>().enabled = true;
                    playerObject.GetComponentInChildren<Outline>().OutlineColor = Color.white;
                    attackablePlayers.Add(playerObject);

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

            currentlyAttackablePlayers = attackablePlayers;
        }

        private void DisableBaseSelection(List<GameObject> attackablePlayers)
        {
            currentlySelectingPlayer = false;
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                PlayerSelectionHandler playerSelectionHandler = player.GetComponent<PlayerSelectionHandler>();
                playerSelectionHandler.enabled = false;

            }
            foreach (GameObject player in playerIdentifier.playerIndex)
            {
                if(player != null)
                {
                    player.GetComponent<PlayerSelectionHandler>().enabled = false;
                    player.GetComponent<Selectable>().enabled = false;
                    player.GetComponentInChildren<Outline>().enabled = false;
                    attackablePlayers.Add(player);
                }
            }

        }

        public void PerformAttack(int damageToDeal)
        {
            playerIdentifier.SetupCurrentPlayerWeapons(playerIdentifier.currentPlayer);

            if (isDoubleRoll) damageToDeal *= 2;

            isDoubleRoll = false;

            damageToDeal += rollBonus;

            currentDamage = damageToDeal;

            AttackCards(damageToDeal);

            int minColour = attack.minDamageFromDice();
            int maxColour = attack.maxDamageFromDice();

            GameObject attackWeapon = null;

            foreach (GameObject weapon in playerIdentifier.currentPlayerWeapons)
            {
                if (weapon.GetComponent<Weapon>().weaponType == weaponClass)
                {
                    attackWeapon = weapon;
                }
            }

            // Start the coroutine and wait until it finishes
            StartCoroutine(WaitForAttack(attackWeapon, damageToDeal, minColour, maxColour));

            rollBonus = 0;
        }

        private void AttackCards(int damageToDeal)
        {
            PlayerCards recieversCards = playerToDamage.GetComponent<PlayerCards>();

            List<Card> cardList = new List<Card>();
            foreach (Card card in recieversCards.cardsPlayed)
            {
                cardList.Add(card);
            }

            foreach (Card card in cardList)
            {
                if (card.hasEffectWhenAttacked)
                {
                    if (card.cardType == Card.CardType.damageReduction)
                    {
                        float _damageToDeal = damageToDeal;
                        _damageToDeal *= card.damageReductionPercentage;
                        damageToDeal = (int)_damageToDeal;
                        currentDamage = damageToDeal;
                        recieversCards.cardsPlayed.Remove(card);
                        recieversCards.RemovePlayedCards(card.CardWorldPreFab);
                        playerToDamage.GetComponent<PlayerSetup>().DeDamageReduction();
                    }
                }
            }

            PlayerCards playersCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            List<Card> _cardList = new List<Card>();
            foreach (Card card in playersCards.cardsPlayed)
            {
                _cardList.Add(card);
            }

            foreach (Card card in _cardList)
            {
                if (card.activationType == Card.ActivationType.onAttack)
                {
                    if (card.cardType == Card.CardType.stun)
                    {
                        recieversCards.cardsPlayedAgainst.Add(card);
                        playersCards.cardsPlayed.Remove(card);
                        playersCards.RemovePlayedCards(card.CardWorldPreFab);
                        playerToDamage.GetComponent<PlayerSetup>().amountOfRoundsStun = card.amountOfRounds;
                    }

                    if (card.cardType == Card.CardType.dot)
                    {
                        recieversCards.cardsPlayedAgainst.Add(card);
                        playersCards.cardsPlayed.Remove(card);
                        playersCards.RemovePlayedCards(card.CardWorldPreFab);
                        playerToDamage.GetComponent<PlayerSetup>().amountOfRoundsDOT = card.amountOfRounds;
                    }

                    if (card.cardType == Card.CardType.aoe)
                    {
                        int targetsIndex = 0;

                        for (int i = 0; i < playerIdentifier.turnOrderIndex.Count; i++)
                        {
                            if (playerIdentifier.turnOrderIndex[i] == playerToDamage) targetsIndex = i;
                        }

                        if (targetsIndex + 1 > playerIdentifier.turnOrderIndex.Count - 1)
                        {
                            playerRight = playerIdentifier.turnOrderIndex[0];
                            if (playerRight.GetComponent<Health>().isDead || playerRight == playerIdentifier.currentPlayer) playerRight = null;
                        }
                        else
                        {
                            playerRight = playerIdentifier.turnOrderIndex[targetsIndex + 1];
                            if (playerRight.GetComponent<Health>().isDead || playerRight == playerIdentifier.currentPlayer) playerRight = null;
                        }

                        if (targetsIndex - 1 < 0)
                        {
                            playerLeft = playerIdentifier.turnOrderIndex[playerIdentifier.turnOrderIndex.Count - 1];
                            if (playerLeft.GetComponent<Health>().isDead || playerLeft == playerIdentifier.currentPlayer) playerLeft = null;
                        }
                        else
                        {
                            playerLeft = playerIdentifier.turnOrderIndex[targetsIndex - 1];
                            if (playerLeft.GetComponent<Health>().isDead || playerLeft == playerIdentifier.currentPlayer) playerLeft = null;
                        }

                        playersCards.cardsPlayed.Remove(card);
                        playersCards.RemovePlayedCards(card.CardWorldPreFab);
                    }
                }
            }
        }

        private IEnumerator WaitForAttack(GameObject attackWeapon, int damageToDeal, int minColour, int maxColour)
        {
            int middlePlayerNum = 0;
            int rightPlayerNum = 0;

            if(playerRight != null)
            {
                middlePlayerNum = 1;
            }

            if(playerLeft != null)
            {
                yield return StartCoroutine(SlerpAttackWeapon(attackWeapon, playerLeft));
                playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().SpawnProjectile(damageToDeal, attackWeapon, playerLeft, minColour, maxColour, this, 2);
                playerLeft = null;
            }

            
            yield return StartCoroutine(SlerpAttackWeapon(attackWeapon, playerToDamage));
            playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().SpawnProjectile(damageToDeal, attackWeapon, playerToDamage, minColour, maxColour, this, middlePlayerNum);


            if (playerRight != null)
            {
                yield return StartCoroutine(SlerpAttackWeapon(attackWeapon, playerRight));
                Debug.Log("here");
                playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().SpawnProjectile(damageToDeal, attackWeapon, playerRight, minColour, maxColour, this, rightPlayerNum);
                playerRight = null;
            }



            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();
            playerEnergy.SpendEnergy(attack.cost);
            scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].damageDealt += damageToDeal;

            physicalDiceCalculator.ClearDice();
        }

        public void RollDice(GameObject toDamage)
        {
            int currentRollBonus = attack.rollBonus;

            PlayerCards playerCards = playerIdentifier.currentPlayer.GetComponent<PlayerCards>();

            List<Card> cardList = new List<Card>();
            foreach (Card card in playerCards.cardsPlayed)
            {
                cardList.Add(card);
            }

            foreach (Card card in cardList)
            {
                if (card.activationType ==  Card.ActivationType.onDiceRoll)
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

            playerToDamage = toDamage;
            diceSpawner.ActivateDice(attack);

            List<GameObject> attackablePlayers = new List<GameObject>();
            DisableBaseSelection(attackablePlayers);
        }

        public void SpawnDamageNumbers(GameObject toDamage, int minColour, int maxColour, bool fromCard, int damage)
        {
            if (fromCard) currentDamage = damage;

            GameObject numbersPrefab = Instantiate(damageNumbersPrefab, toDamage.transform.position, quaternion.identity);
            numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = GetColorOnGradient(currentDamage, minColour, maxColour, colourGradient);
            NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
            numbersDisplay.SpawnNumbers(currentDamage);
        }

        public Color GetColorOnGradient(int value, int minValue, int maxValue, Gradient colorGradient)
        {
            float position = (float)(value - minValue) / (maxValue - minValue);
            return colorGradient.Evaluate(position);
        }

        private IEnumerator SlerpAttackWeapon(GameObject attackWeapon, GameObject playerToTarget)
        {
            Quaternion initialRotation = attackWeapon.transform.rotation;

            Vector3 targetPosition = playerToTarget.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - attackWeapon.transform.position);

            float elapsedTime = 0f;
            float rotationTime = 1f;

            while (elapsedTime < rotationTime)
            {
                attackWeapon.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotationTime);
                elapsedTime += Time.deltaTime;
                if (playerToTarget == playerRight)
                {
                    Debug.Log(elapsedTime);
                }
                yield return null;
            }
            //attackWeapon.transform.rotation = initialRotation;
        }
    }
}