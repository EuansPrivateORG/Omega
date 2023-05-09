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

        public List<GameObject> playerImageList = new List<GameObject>();

        private List<GameObject> playerImageListPrivate = new List<GameObject>();

        private GameObject instantiatedPlayer;

        private PlayerSetup playersSetup;

        private int playerCounter;

        private void Awake()
        {
            playerIdentifier = GetComponent<PlayerIdentifier>();

        }

        public void StartFirstRound(List<Base> playersToSpawn)
        {
            foreach (Base player in playersToSpawn)
            {
                playersToSpawnIn.Add(player);
            }

            float angleBetweenPoints = 360f / numberOfPlayers;
            Vector3 centerPosition = transform.position;

            ShuffleList(playersToSpawnIn);

            for (int i = playersToSpawnIn.Count - 1; i >= 0; i--)
            {
                playerCounter++;
                float angle = i * angleBetweenPoints; // change angle calculation to anti-clockwise
                Vector3 position = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), 0f, radius * Mathf.Sin(angle * Mathf.Deg2Rad));

                instantiatedPlayer = Instantiate(playersToSpawnIn[i].emptyPreFab, position, Quaternion.identity);
                playersSetup = instantiatedPlayer.GetComponent<PlayerSetup>();
                playersSetup.playerID = playerCounter;
                Debug.Log(playerCounter);
                CreatIcon();
                GetComponent<ScoreHandler>().AddScorer(playersToSpawn[i], playerCounter);
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

        public void StartNextRound(List<Base> playersToSpawn)
        {
            foreach(Base player in playersToSpawn)
            {
                playersToSpawnIn.Add(player);
            }
            float angleBetweenPoints = 360f / numberOfPlayers;
            Vector3 centerPosition = transform.position;

            ShuffleList(playersToSpawnIn);

            Debug.Log("Amount of Players: " + playersToSpawnIn.Count);

            for (int i = playersToSpawnIn.Count - 1; i >= 0; i--)
            {
                playerCounter++;
                float angle = i * angleBetweenPoints; // change angle calculation to anti-clockwise
                Vector3 position = new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), 0f, radius * Mathf.Sin(angle * Mathf.Deg2Rad));

                instantiatedPlayer = Instantiate(playersToSpawnIn[i].emptyPreFab, position, Quaternion.identity);
                playersSetup = instantiatedPlayer.GetComponent<PlayerSetup>();
                playersSetup.playerID = playerCounter;
                CreatIcon();
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

        private void CreatIcon()
        {
            GameObject instantiatedPlayerIcon = Instantiate(playersSetup.playerBase.turnOrderVarientIcon, playersTurnOrder);
            instantiatedPlayerIcon.name = ("Player Icon" + (playersSetup.playerID));
            playerImageList.Add(instantiatedPlayerIcon);
            playerImageListPrivate.Add(instantiatedPlayerIcon);
        }

        public static void ShuffleList<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                T temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }

        public void ResetPlayers()
        {
            foreach(GameObject player in playerList)
            {
                Destroy(player);
            }

            foreach (GameObject icon in playerImageListPrivate)
            {
                Destroy(icon);
            }

            playerCounter = 0;
            playerImageListPrivate.Clear();
            playerImageList.Clear();
            playerList.Clear();
            playerIdentifier.ResetIndex();
        }
    }
}
