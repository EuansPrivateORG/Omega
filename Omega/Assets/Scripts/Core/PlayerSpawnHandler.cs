using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Status;
using UnityEngine.UI;
using Omega.UI;

namespace Omega.Core
{
    public class PlayerSpawnHandler : MonoBehaviour
    {
        [Header("Spawnable")]
        [Tooltip("Base Player Object")]


        [SerializeField] public GameObject basePlayerTurnOrderPrefab;


        [Header("PlayerCount")]
        [Range(3,6)]
        [Tooltip("Current number of players in game session")]
        [SerializeField] public int numberOfPlayers ;                                     //Need to linked into the main menu where we assign the amount of players in the game

        [Header("Stats")]
        [Tooltip("Health all players will spawn with")]
        [SerializeField] private int playerStartingHealth;

        [Tooltip("Energy all players will spawn with")]
        [SerializeField] private int playerStartingEnergy;

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

        public List<Base> playersToSpawnIn = new List<Base>();

        PlayerIdentifier playerIdentifier;

        public List<GameObject> playerImageList;

        private GameObject instantiatedPlayer;

        private PlayerSetup playersSetup;

        private int playerCounter;

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();

        }




        public void SpawnPlayers(List<Base> playersToSpawn)
        {
            playersToSpawnIn = playersToSpawn;
            float angleBetweenPoints = 360f / numberOfPlayers;
            Vector3 centerPosition = transform.position;


            for (int i = playersToSpawnIn.Count - 1; i >= 0; i--)
            {
                playerCounter++;
                float angle = i * angleBetweenPoints; // change angle calculation to anti-clockwise
                Vector3 position = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), 0f, radius * Mathf.Sin(angle * Mathf.Deg2Rad));

                instantiatedPlayer = Instantiate(playersToSpawnIn[i].emptyPreFab, position, Quaternion.identity);
                playersSetup = instantiatedPlayer.GetComponent<PlayerSetup>();
                playersSetup.playerID = playerCounter;
                CreatIcon(i);
                instantiatedPlayer.transform.SetParent(players);
                instantiatedPlayer.transform.LookAt(centerPosition);
                instantiatedPlayer.name = ("Player " + (playerCounter)).ToString();
                playerList.Add(instantiatedPlayer);

                instantiatedPlayer.GetComponent<Health>().maxHealth = playerStartingHealth;
                instantiatedPlayer.GetComponent<Health>().SetHealth();
                instantiatedPlayer.GetComponent<Energy>().energy = playerStartingEnergy;

                playersToSpawnIn.RemoveAt(i);
            }
            playerIdentifier.SetIndex(playerList);
            CameraHandler cameraHandler = FindObjectOfType<CameraHandler>();
            cameraHandler.SetupCameras();
            TurnTransition turnTransition = FindObjectOfType<TurnTransition>();
            StartCoroutine(turnTransition.FadeInHUD());
        }

        private void CreatIcon(int i)
        {
            GameObject instantiatedPlayerIcon = Instantiate(playersSetup.playerBase.turnOrderVarientIcon, playersTurnOrder);
            instantiatedPlayerIcon.name = ("Player Icon" + (playersSetup.playerID));
            playerImageList.Add(instantiatedPlayerIcon);
        }
    }
}
