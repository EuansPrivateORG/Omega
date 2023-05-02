using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Status;
using UnityEngine.UI;

namespace Omega.Core
{
    public class PlayerSpawnHandler : MonoBehaviour
    {
        [Header("Spawnable")]
        [Tooltip("Base Player Object")]
        [SerializeField] public GameObject basePlayerPrefab;
        [SerializeField] public GameObject basePlayerTurnOrderPrefab;


        [Header("PlayerCount")]
        [Tooltip("Current number of players in game session")]
        [SerializeField] public int numberOfPlayers;                                     //Need to linked into the main menu where we assign the amount of players in the game

        [Header("Stats")]
        [Tooltip("Health all players will spawn with")]
        [SerializeField] private int health;

        [Tooltip("Energy all players will spawn with")]
        [SerializeField] private int energy;

        [Header("Spawning Radius")]
        [Tooltip("the size of the circle the players are spawned on")]
        [SerializeField] public float radius;                                           //conistant for map size

        [Header("Hierachy Control")]
        [Tooltip("Where in the hierachy the players are spawned")]
        [SerializeField] Transform players;

        [Tooltip("Where in the hierachy the players Turn Order Icons are spawned")]
        [SerializeField] RectTransform playersTurnOrder;

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
            SpawnPlayerTurnOrder();
        }

        private void SpawnPlayers()
        {
            float angleBetweenPoints = 360f / numberOfPlayers;
            Vector3 centerPosition = transform.position;

            for (int i = 0; i < numberOfPlayers; i++)
            {
                float angle = 360f - i * angleBetweenPoints; // change angle calculation to clockwise
                Vector3 position = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), 0f, radius * Mathf.Sin(angle * Mathf.Deg2Rad));
                GameObject instantiatedPlayer = Instantiate(basePlayerPrefab, position, Quaternion.identity);
                instantiatedPlayer.transform.SetParent(players);
                instantiatedPlayer.transform.LookAt(centerPosition);
                instantiatedPlayer.name = ("Player " + (i + 1)).ToString();
                playerList.Add(instantiatedPlayer);
                playerIdentifier.SetIndex(playerList);

                instantiatedPlayer.GetComponent<Health>().maxHealth = health;
                instantiatedPlayer.GetComponent<Energy>().energy = energy;

                
            }
            CameraHandler cameraHandler = FindObjectOfType<CameraHandler>();
            cameraHandler.SetupCameras();
        }

        private void SpawnPlayerTurnOrder()
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                GameObject instantiatedPlayerIcon = Instantiate(basePlayerTurnOrderPrefab, playersTurnOrder);
                //this should be moved to its own script where we handle the turn order transitions based on the currenly player from the identifier
                if(i == playerIdentifier.currentPlayerIndex)
                {
                    instantiatedPlayerIcon.GetComponent<Image>().enabled = true;
                }
            }
        }

    }
}

