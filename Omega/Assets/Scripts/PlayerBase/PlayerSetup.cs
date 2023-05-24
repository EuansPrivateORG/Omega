using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Omega.Status;

namespace Omega.Core
{
    public class PlayerSetup : MonoBehaviour
    {
        public Base playerBase;

        [HideInInspector] public GameObject playerPreFab;

        public int playerID = 0;

        private PlayerSpawnHandler playerSpawnHandler;
        public Transform diceTarget;
        public Transform diceSpawn;

        private GameObject playerShield = null;
        private GameObject playerDamageReduction = null;

        [HideInInspector] public int amountOfRoundsDOT;
        [HideInInspector] public int amountOfRoundsHOT;
        [HideInInspector] public int amountOfRoundsShield;
        [HideInInspector] public int amountOfRoundsStun;



        private void Awake()
        {
            playerSpawnHandler = FindObjectOfType<PlayerSpawnHandler>();
        }

        void Start()
        {
            playerPreFab = playerBase.baseVarientPrefabList[Random.Range(0, playerBase.baseVarientPrefabList.Count)];
            GameObject instantiated = Instantiate(playerPreFab, transform);

            MeshRenderer playerMesh = instantiated.GetComponent<MeshRenderer>();
            playerMesh.material = playerBase.materialVarientOverrite;
        }

        public void SetPlayerIconDead()
        {
            if (FindObjectOfType<PlayerIdentifier>().currentlyAlivePlayers.Count > 1)
            {
                PlayerIconID playerIconID = playerSpawnHandler.playerImageList[playerID - 1].GetComponent<PlayerIconID>();
                playerIconID.iconBackground.SetActive(false);
                playerIconID.destroyedIconBackground.SetActive(true);
                playerIconID.playerIcon.GetComponent<Image>().color = playerIconID.destroyedIconBackground.GetComponent<Image>().color;
                playerSpawnHandler.playerImageList.Remove(playerSpawnHandler.playerImageList[playerID - 1]);
            }
        }

        public void UpdatePlayerID()
        {
            playerID -= 1;
        }

        public void ActivateShield(GameObject shield)
        {
            playerShield = Instantiate(shield, transform);
        }

        public void ActivateDamageReduction(GameObject shield)
        {
            playerDamageReduction = Instantiate(shield, transform);
        }

        public void DeActivateShield()
        {
            Destroy(playerShield);
        }

        public void DeDamageReduction()
        {
            Destroy(playerDamageReduction);
        }
    }
}