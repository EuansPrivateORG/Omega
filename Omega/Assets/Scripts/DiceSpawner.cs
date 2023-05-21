using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Core;

namespace Omega.Actions
{
    public class DiceSpawner : MonoBehaviour
    {
        [Header("Dice Prefabs")]
        [SerializeField] public GameObject D4;
        [SerializeField] public GameObject D6;
        [SerializeField] public GameObject D20;

        public float spawnRadius;

        private PlayerIdentifier playerIdentifier;
        private List<GameObject> dice = new List<GameObject>();
        private PhysicalDiceCalculator diceCalculator;

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();
            diceCalculator = GetComponent<PhysicalDiceCalculator>();
        }

        public void ActivateChaosDice()
        {
            PlayerSetup playerSetup = playerIdentifier.currentPlayer.GetComponent<PlayerSetup>();

            SpawnDice(1, playerSetup, D20);
        }

        public void ActivateDice(PlayerAction action)
        {
            diceCalculator.passedInfo = false;

            PlayerSetup playerSetup = playerIdentifier.currentPlayer.GetComponent<PlayerSetup>();

            SpawnDice(action.numOfD4, playerSetup, D4);

            SpawnDice(action.numOfD6, playerSetup, D6);

            foreach (GameObject d in dice) 
            {
                d.GetComponent<PhysicalDice>().RollDice(playerSetup.diceTarget);
            }
            dice.Clear();
        }

        private void SpawnDice(int numOfDice, PlayerSetup playerSetup, GameObject diceToSpawn)
        {
            for (int i = 0; i < numOfDice; i++)
            {
                Vector3 spawnPosition = playerSetup.diceSpawn.position;
                Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f); // Check for any colliders within 0.5 units of the spawn position
                bool foundColliders = colliders.Length > 0;

                if (foundColliders)
                {
                    spawnPosition = GetAdjacentPosition(spawnPosition); // Get an adjacent position if there are any colliders at the spawn position
                }

                GameObject instantiated = Instantiate(diceToSpawn, spawnPosition, Quaternion.identity);
                dice.Add(instantiated);
            }
        }

        private Vector3 GetAdjacentPosition(Vector3 position)
        {
            Vector3 adjacentPosition = position;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (x == 0 && y == 0 && z == 0) continue; // Skip the current position

                        Vector3 offset = new Vector3(x, y, z);
                        Vector3 adjacent = position + offset;
                        Collider[] colliders = Physics.OverlapSphere(adjacent, 0.5f); // Check for any colliders within 0.5 units of the adjacent position

                        if (colliders.Length == 0)
                        {
                            adjacentPosition = adjacent; // Use this adjacent position if there are no colliders
                            break;
                        }
                    }
                }
            }

            return adjacentPosition;
        }

    }
}

