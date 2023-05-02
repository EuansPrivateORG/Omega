using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Status;


namespace Omega.Core
{
    public class PlayerSpawnHandler : MonoBehaviour
    {
        [Header("Spawnable")]
        [Tooltip("Base Player Object")]
        [SerializeField] public GameObject basePlayerPrefab;

        [Header("PlayerCount")]
        [Tooltip("Current number of players in game session")]
        [SerializeField] public int numberOfPlayers;                    //Need to linked into the main menu where we assign the amount of players in the game

        [Header("Stats")]

        [Tooltip("Health all players will spawn with")]
        [SerializeField] private int health;

        [Tooltip("Energy all players will spawn with")]
        [SerializeField] private int energy;

        [Header("Spawning Radius")]
        [Tooltip("the size of the circle the players are spawned on")]
        [SerializeField] public float radius;                           //conistant for map size

        [Header("Hierachy Control")]
        [Tooltip("Where in the hierachy the players are spawned")]
        [SerializeField] Transform players;

        [HideInInspector]
        [SerializeField] private List<GameObject> playerList = new List<GameObject>();

        PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();
        }
        void Start()
        {
            SpawnPlayers();
        }

        private void SpawnPlayers()
        {
            float angleBetweenPoints = 360f / numberOfPlayers;
            Vector3 centerPosition = transform.position;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                float angle = i * angleBetweenPoints;
                Vector3 position = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), 0f, radius * Mathf.Sin(angle * Mathf.Deg2Rad));
                GameObject spawnedObject = Instantiate(basePlayerPrefab, position, Quaternion.identity);
                spawnedObject.transform.SetParent(players);
                spawnedObject.transform.LookAt(centerPosition);
                spawnedObject.name = ("Player " + (i+1)).ToString(); 
                playerList.Add(spawnedObject);
                playerIdentifier.SetIndex(playerList);

                spawnedObject.GetComponent<Health>().maxHealth = health;
                spawnedObject.GetComponent<Energy>().energy = energy;
            }
        }
    }
}

