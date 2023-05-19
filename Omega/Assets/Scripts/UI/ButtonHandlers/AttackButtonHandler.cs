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

        private CancelHandler cancelHandler;

        [HideInInspector] public bool currentlySelectingPlayer = false;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            diceSpawner = FindObjectOfType<DiceSpawner>();
            playerHUD = FindObjectOfType<RoundHandler>().playerHUD;
            cancelHandler = GetComponent<CancelHandler>();

        }

        private void OnEnable()
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
        private void OnDisable()
        {
            List<GameObject> attackablePlayers = new List<GameObject>();
            DisableBaseSelection(attackablePlayers);
        }

        public void ButtonPressed()
        {
            playerIdentifier.isAttacking = true;

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
            damageToDeal += attack.rollBonus;
            currentDamage = damageToDeal;

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

            Debug.Log("this attack uses this:" + attackWeapon.name);

            // Start the coroutine and wait until it finishes
            StartCoroutine(WaitForAttack(attackWeapon, damageToDeal, minColour, maxColour));
        }

        private IEnumerator WaitForAttack(GameObject attackWeapon, int damageToDeal, int minColour, int maxColour)
        {
            yield return StartCoroutine(SlerpAttackWeapon(attackWeapon));

            playerIdentifier.currentPlayer.GetComponent<ProjectileSpawner>().SpawnProjectile(damageToDeal, attackWeapon, playerToDamage, minColour, maxColour, this);

            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();
            playerEnergy.SpendEnergy(attack.cost);
            scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].damageDealt += damageToDeal;
        }

        public void RollDice(GameObject toDamage)
        {
            playerIdentifier.currentAttack = this;
            playerToDamage = toDamage;
            diceSpawner.ActivateDice(attack);

            List<GameObject> attackablePlayers = new List<GameObject>();
            DisableBaseSelection(attackablePlayers);
        }

        public void SpawnDamageNumbers(GameObject toDamage, int minColour, int maxColour)
        {
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

        private IEnumerator SlerpAttackWeapon(GameObject attackWeapon)
        {
            Quaternion initialRotation = attackWeapon.transform.rotation;

            Vector3 targetPosition = playerToDamage.transform.position;
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