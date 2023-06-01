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
using Unity.VisualScripting;

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

        AttackButtonID attackButtonID;

        [HideInInspector] public List<GameObject> currentlyAttackablePlayers;

        [HideInInspector] public bool projectileIsFiring = false;
        private GameObject currentAttackWeapon = null;

        private void Awake()
        {
            attackButtonID = FindObjectOfType<AttackButtonID>();
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            diceSpawner = FindObjectOfType<DiceSpawner>();
            playerHUD = FindObjectOfType<RoundHandler>().playerHUD;
            cancelHandler = GetComponent<CancelHandler>();
            numberRoller = FindObjectOfType<NumberRoller>();
            physicalDiceCalculator = FindObjectOfType<PhysicalDiceCalculator>();
        }
        private void Update()
        {
            if (projectileIsFiring)
            {
                currentAttackWeapon.transform.LookAt(playerToDamage.transform);
            }
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

                    int placeInList = 0;

                    for (int i = 0; i < attackButtonID.attacks.Count; i++)
                    {
                        if (attackButtonID.attacks[i] == this)
                        {
                            placeInList = i; break;
                        }
                    }
                    if(placeInList != attackButtonID.attacks.Count - 1)
                    {
                        Button thisButton = GetComponent<Button>();

                        if (playerEnergy.energy < attackButtonID.attacks[placeInList + 1].attack.cost)
                        {
                            Navigation newNav = new Navigation();
                            newNav.mode = Navigation.Mode.Explicit;
                            if (placeInList != 0) newNav.selectOnLeft = attackButtonID.attacks[placeInList - 1].GetComponent<Button>();
                            thisButton.navigation = newNav;
                        }
                        else
                        {
                            Navigation newNav = new Navigation();
                            newNav.mode = Navigation.Mode.Explicit;
                            newNav.selectOnRight = attackButtonID.attacks[placeInList + 1].GetComponent<Button>();
                            if (placeInList != 0) newNav.selectOnLeft = attackButtonID.attacks[placeInList - 1].GetComponent<Button>();
                            thisButton.navigation = newNav;
                        }
                    }
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
                    playerObject.GetComponent<PlayerSelectionHandler>().enabled = true;
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

            foreach (GameObject playerObject in playerIdentifier.currentlyAlivePlayersInTurn)
            {
                BaseSelectionHandlerOverwrite baseSelectionHandlerOverwrite = playerObject.GetComponent<BaseSelectionHandlerOverwrite>();
                baseSelectionHandlerOverwrite.playersToChooseFrom = currentlyAttackablePlayers;
                baseSelectionHandlerOverwrite.InitialNav();
            }
        }

        private void DisableBaseSelection(List<GameObject> attackablePlayers)
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
            StartCoroutine(WaitForAttack(attackWeapon, currentDamage, minColour, maxColour));

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

                    if (card.cardType == Card.CardType.stun)
                    {
                        if(playerLeft != null)
                        {
                            playerLeft.GetComponent<PlayerCards>().cardsPlayedAgainst.Add(card);
                            playerLeft.GetComponent<PlayerSetup>().amountOfRoundsStun = card.amountOfRounds;
                            playerLeft.GetComponent<BaseVFX>().StartStunVFX();
                        }
                        if (playerRight != null)
                        {
                            playerRight.GetComponent<PlayerCards>().cardsPlayedAgainst.Add(card);
                            playerRight.GetComponent<PlayerSetup>().amountOfRoundsStun = card.amountOfRounds;
                            playerRight.GetComponent<BaseVFX>().StartStunVFX();
                        }

                        recieversCards.cardsPlayedAgainst.Add(card);
                        playersCards.cardsPlayed.Remove(card);
                        playersCards.RemovePlayedCards(card.CardWorldPreFab);
                        playerToDamage.GetComponent<PlayerSetup>().amountOfRoundsStun = card.amountOfRounds;
                        playerToDamage.GetComponent<BaseVFX>().StartStunVFX();
                    }

                    if (card.cardType == Card.CardType.dot)
                    {
                        if (playerLeft != null)
                        {
                            playerLeft.GetComponent<PlayerCards>().cardsPlayedAgainst.Add(card);
                            playerLeft.GetComponent<PlayerSetup>().amountOfRoundsStun = card.amountOfRounds;
                            playerToDamage.GetComponent<BaseVFX>().StartDamageVFX();
                        }
                        if (playerRight != null)
                        {
                            playerLeft.GetComponent<PlayerCards>().cardsPlayedAgainst.Add(card);
                            playerLeft.GetComponent<PlayerSetup>().amountOfRoundsStun = card.amountOfRounds;
                            playerToDamage.GetComponent<BaseVFX>().StartDamageVFX();
                        }

                        recieversCards.cardsPlayedAgainst.Add(card);
                        playersCards.cardsPlayed.Remove(card);
                        playersCards.RemovePlayedCards(card.CardWorldPreFab);
                        playerToDamage.GetComponent<PlayerSetup>().amountOfRoundsDOT = card.amountOfRounds;
                        playerToDamage.GetComponent<BaseVFX>().StartDamageVFX();
                    }

                    if(card.cardType == Card.CardType.lifeSteal)
                    {
                        HealingButtonHandler health = FindObjectOfType<HealingButtonHandler>();
                        float amountToHeal = damageToDeal * card.lifeStealPercentage;
                        health.PerformHealing((int)amountToHeal, card, playerIdentifier.currentPlayer);
                        playerIdentifier.currentPlayer.GetComponent<BaseVFX>().PerformHealing();
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
                playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().SpawnProjectile(ShieldDamage(playerLeft, damageToDeal), attackWeapon, playerLeft, minColour, maxColour, this, 2);
                playerLeft = null;
                LookAtPlayer(attackWeapon);
            }


            yield return StartCoroutine(SlerpAttackWeapon(attackWeapon, playerToDamage));
            playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().SpawnProjectile(ShieldDamage(playerToDamage, damageToDeal), attackWeapon, playerToDamage, minColour, maxColour, this, middlePlayerNum);
            LookAtPlayer(attackWeapon);

            if (playerRight != null)
            {
                yield return StartCoroutine(SlerpAttackWeapon(attackWeapon, playerRight));
                playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().SpawnProjectile(ShieldDamage(playerRight, damageToDeal), attackWeapon, playerRight, minColour, maxColour, this, rightPlayerNum);
                playerRight = null;
                LookAtPlayer(attackWeapon);
            }



            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();
            playerEnergy.SpendEnergy(attack.cost);
            scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].damageDealt += damageToDeal;

            physicalDiceCalculator.ClearDice();
        }

        private void LookAtPlayer(GameObject attackWeapon)
        {
            currentAttackWeapon = attackWeapon;
            if (currentAttackWeapon.GetComponent<Weapon>().weaponType != Weapon.weaponClass.Ultimate)
            {
                projectileIsFiring = true;
            }
        }

        private int ShieldDamage(GameObject _playerToDamage, int damageToDeal)
        {
            damageToDeal = currentDamage;
            PlayerCards recieversCards = _playerToDamage.GetComponent<PlayerCards>();

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
                        float newDam = damageToDeal * card.damageReductionPercentage;
                        damageToDeal = (int)newDam;
                        recieversCards.cardsPlayed.Remove(card);
                        recieversCards.RemovePlayedCards(card.CardWorldPreFab);
                        _playerToDamage.GetComponent<PlayerSetup>().DeDamageReduction();
                    }
                }
            }

            return damageToDeal;
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
            //if (fromCard) currentDamage = damage;

            GameObject numbersPrefab = Instantiate(damageNumbersPrefab, toDamage.transform.position, quaternion.identity);
            numbersPrefab.GetComponentInChildren<TextMeshProUGUI>().color = GetColorOnGradient(damage, minColour, maxColour, colourGradient);
            NumbersDisplay numbersDisplay = numbersPrefab.gameObject.GetComponent<NumbersDisplay>();
            numbersDisplay.SpawnNumbers(damage);
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
                yield return null;
            }
            //attackWeapon.transform.rotation = initialRotation;
        }
    }
}