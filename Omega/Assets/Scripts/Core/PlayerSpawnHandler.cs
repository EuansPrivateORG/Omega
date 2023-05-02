using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

        [Header("Spawning Radius")]
        [Tooltip("the size of the circle the players are spawned on")]
        [SerializeField] public float radius;                           //conistant for map size

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
                spawnedObject.transform.LookAt(centerPosition);

            }
        }
    }
}
