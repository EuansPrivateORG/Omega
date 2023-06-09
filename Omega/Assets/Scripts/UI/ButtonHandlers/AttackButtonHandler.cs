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
using JetBrains.Annotations;

namespace Omega.UI
{
    public class AttackButtonHandler : ActionButtonHandler
    {
        public Color unselectedOutlineColor;

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
        private GameObject currentTarget = null;

        [HideInInspector] public bool continueWithAttack = true;

        private bool hunterThisTurn = false;
        private List<GameObject> playersWithHunter = new List<GameObject>();

        private bool availableSelection = true;

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
                currentAttackWeapon.transform.LookAt(currentTarget.transform);
            }
        }

        private void OnEnable()
        {
            if (!isAttackButton)
            {
                cancelHandler.cannotCancel = false;

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
                    playerObject.GetComponentInChildren<Outline>().OutlineColor = unselectedOutlineColor;
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
                        playersCards.cardsPlayed.Remove(card);
                        playersCards.RemovePlayedCards(card.CardWorldPreFab);
                    }

                    if (card.cardType == Card.CardType.huntersMark)
                    {
                        if (playerLeft != null)
                        {
                            playerLeft.GetComponent<PlayerCards>().cardsPlayedAgainst.Add(card);
                            playersWithHunter.Add(playerLeft);
                            playerLeft.GetComponent<BaseVFX>().StartReticle();
                        }
                        if (playerRight != null)
                        {
                            playerLeft.GetComponent<PlayerCards>().cardsPlayedAgainst.Add(card);
                            playersWithHunter.Add(playerRight);
                            playerRight.GetComponent<BaseVFX>().StartReticle();
                        }

                        recieversCards.cardsPlayedAgainst.Add(card);
                        playersCards.cardsPlayed.Remove(card);
                        playersCards.RemovePlayedCards(card.CardWorldPreFab);
                        playersWithHunter.Add(playerToDamage);
                        playerToDamage.GetComponent<BaseVFX>().StartReticle();
                        hunterThisTurn = true;
                    }

                    if (card.cardType == Card.CardType.sacrafice)
                    {
                        //Dealing Damage to player
                        float sacraficeDamPlayer = card.sacraficeLoss * damageToDeal;
                        playerIdentifier.currentPlayer.GetComponent<Health>().TakeDamage((int)sacraficeDamPlayer);
                        SpawnDamageNumbers(playerIdentifier.currentPlayer, (int)sacraficeDamPlayer - 5, (int)sacraficeDamPlayer + 5, true, (int)sacraficeDamPlayer);
                        playerIdentifier.currentPlayer.GetComponent<DamageStateCollection>().CheckHealth();
                        if (playerIdentifier.currentPlayer.GetComponent<Health>().isDead)
                        {
                            playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().playersToStopAttack.Add(playerIdentifier.currentPlayer);
                        }

                        CardHandler cardHandler = FindObjectOfType<CardHandler>();
                        cardHandler.cardPlayedSource.clip = cardHandler.takeDamageSound;
                        cardHandler.cardPlayedSource.Play();

                        //Uping damage for other players
                        float sacraficeDam = damageToDeal * card.sacraficeGain;
                        currentDamage = (int)sacraficeDam;
                        
                        playersCards.cardsPlayed.Remove(card);
                        playersCards.RemovePlayedCards(card.CardWorldPreFab);
                        playerIdentifier.currentPlayer.GetComponent<BaseVFX>().StartSacraficeVFX();
                    }
                }
            }
        }

        private IEnumerator WaitForAttack(GameObject attackWeapon, int damageToDeal, int minColour, int maxColour)
        {
            continueWithAttack = true;
            int middlePlayerNum = 0;
            int rightPlayerNum = 0;

            Disruptor(playerToDamage, damageToDeal);

            if (playerLeft != null)
            {
                Disruptor(playerLeft, damageToDeal);
            }
            if (playerRight != null)
            {
                Disruptor(playerRight, damageToDeal);
            }

            if (playerRight != null)
            {
                middlePlayerNum = 1;
            }

            if(playerLeft != null)
            {
                yield return StartCoroutine(SlerpAttackWeapon(attackWeapon, playerLeft));
                continueWithAttack = false;
                ProjectileSpawner _projectileSpawner = playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>();
                _projectileSpawner.SpawnProjectile(RecieverDamage(playerLeft, currentDamage), attackWeapon, playerLeft, minColour, maxColour, this, 2);
                currentTarget = playerLeft;
                playerLeft = null;
                if (_projectileSpawner.playersToStopAttack.Contains(playerLeft))
                {
                    _projectileSpawner.playersToStopAttack.Remove(playerLeft);
                }
                else
                {
                    LookAtPlayer(attackWeapon);
                }
            }
            while (!continueWithAttack)
            {
                yield return null;
            }


            yield return StartCoroutine(SlerpAttackWeapon(attackWeapon, playerToDamage));
            if (playerLeft != null || playerRight != null) continueWithAttack = false;
            else continueWithAttack = true;
            ProjectileSpawner projectileSpawner = playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>();
            projectileSpawner.SpawnProjectile(RecieverDamage(playerToDamage, currentDamage), attackWeapon, playerToDamage, minColour, maxColour, this, middlePlayerNum);
            currentTarget = playerToDamage;
            if (projectileSpawner.playersToStopAttack.Contains(playerToDamage))
            {
                projectileSpawner.playersToStopAttack.Remove(playerToDamage);
            }
            else
            {
                LookAtPlayer(attackWeapon);
            }
            while (!continueWithAttack)
            {
                yield return null;
            }

            if (playerRight != null)
            {
                yield return StartCoroutine(SlerpAttackWeapon(attackWeapon, playerRight));
                ProjectileSpawner __projectileSpawner = playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>();
                __projectileSpawner.SpawnProjectile(RecieverDamage(playerRight, currentDamage), attackWeapon, playerRight, minColour, maxColour, this, rightPlayerNum);
                currentTarget = playerRight;
                playerRight = null;
                if (__projectileSpawner.playersToStopAttack.Contains(playerRight))
                {
                    __projectileSpawner.playersToStopAttack.Remove(playerRight);
                }
                else
                {
                    LookAtPlayer(attackWeapon);
                }
                continueWithAttack = true;
            }



            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();
            playerEnergy.SpendEnergy(attack.cost);
            scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].damageDealt += damageToDeal;

            physicalDiceCalculator.ClearDice();
            hunterThisTurn = false;
            playersWithHunter = new List<GameObject>();
        }

        private void LookAtPlayer(GameObject attackWeapon)
        {
            currentAttackWeapon = attackWeapon;
            if (currentAttackWeapon.GetComponent<Weapon>().weaponType != Weapon.weaponClass.Ultimate)
            {
                projectileIsFiring = true;
            }
        }

        private int RecieverDamage(GameObject _playerToDamage, int damageToDeal)
        {
            bool canGetThrough = true;

            PlayerCards recieversCards = _playerToDamage.GetComponent<PlayerCards>();

            List<Card> cardList = new List<Card>();
            foreach (Card card in recieversCards.cardsPlayed)
            {
                cardList.Add(card);
            }

            cardList.AddRange(recieversCards.cardsPlayedAgainst);

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

                    if (card.cardType == Card.CardType.huntersMark)
                    {
                        if (hunterThisTurn)
                        {
                            canGetThrough = false;
                        }

                        foreach(GameObject player in playersWithHunter)
                        {
                            if(player == _playerToDamage)
                            {
                                int amountOfHunters = 0;
                                foreach (Card _card in recieversCards.cardsPlayedAgainst)
                                { 
                                    if(_card == card)
                                    {
                                        amountOfHunters++;
                                    }
                                }
                                if(amountOfHunters >= 2)
                                {
                                    canGetThrough = true;
                                }
                            } 
                        }

                        if (canGetThrough)
                        {
                            float newDam = damageToDeal * card.huntersMarkPercentage;
                            damageToDeal = (int)newDam;
                            recieversCards.cardsPlayedAgainst.Remove(card);
                            _playerToDamage.GetComponent<BaseVFX>().StopReticle();
                        }
                    }
                }
            }
            return damageToDeal;
        }

        private void Disruptor(GameObject _playerToDamage, int damageToDeal)
        {
            PlayerCards recieversCards = _playerToDamage.GetComponent<PlayerCards>();

            List<Card> cardList = new List<Card>();
            foreach (Card card in recieversCards.cardsPlayed)
            {
                cardList.Add(card);
            }

            cardList.AddRange(recieversCards.cardsPlayedAgainst);

            currentDamage = damageToDeal;
            bool isPlayerLeft = false;
            bool isPlayerRight = false;
            if (_playerToDamage == playerLeft) isPlayerLeft = true;
            if (_playerToDamage == playerRight) isPlayerRight = true;

            foreach (Card card in cardList)
            {
                if (card.cardType == Card.CardType.disruptor)
                {
                    float newDam = damageToDeal * card.disruptorPerc;
                    currentDamage = (int)newDam;

                    recieversCards.cardsPlayed.Remove(card);
                    recieversCards.RemovePlayedCards(card.CardWorldPreFab);
                    playerToDamage.GetComponent<BaseVFX>().StopDisruptor();

                    if (playerLeft != null && playerRight != null)
                    {
                        if (playerIdentifier.currentlyAlivePlayers.Count <= 4)
                        {
                            availableSelection = false;
                        }
                    }
                    else if (playerLeft != null)
                    {
                        if (playerIdentifier.currentlyAlivePlayers.Count <= 3)
                        {
                            availableSelection = false;
                        }
                    }
                    else if (playerRight != null)
                    {
                        if (playerIdentifier.currentlyAlivePlayers.Count <= 3)
                        {
                            availableSelection = false;
                        }
                    }
                    else if (playerIdentifier.currentlyAlivePlayers.Count <= 2)
                    {
                        availableSelection = false;
                    }

                    if (availableSelection)
                    {
                        List<GameObject> playerTargets = new List<GameObject>();
                        playerTargets.AddRange(playerIdentifier.currentlyAlivePlayers);
                        playerTargets.Remove(playerToDamage);
                        playerTargets.Remove(playerIdentifier.currentPlayer);
                        if(playerLeft != null)
                        {
                            playerTargets.Remove(playerLeft);
                        }
                        if (playerRight != null)
                        {
                            playerTargets.Remove(playerRight);
                        }

                        int ran = UnityEngine.Random.Range(0, playerTargets.Count - 1);

                        if (isPlayerLeft)
                        {
                            playerLeft = playerIdentifier.currentlyAlivePlayers[ran];
                        }

                        else if (isPlayerRight)
                        {
                            playerRight = playerIdentifier.currentlyAlivePlayers[ran];
                        }

                        else
                        {
                            playerToDamage = playerIdentifier.currentlyAlivePlayers[ran];
                        }
                    }
                    else
                    {
                        playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().playersToStopAttack.Add(_playerToDamage);
                        Debug.Log("Stopped Attack");
                    }
                }
            }
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
            if(attackWeapon.GetComponent<Weapon>().weaponType != Weapon.weaponClass.Ultimate)
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
            }
            else
            {
                yield return null;
            }
        }
    }
}