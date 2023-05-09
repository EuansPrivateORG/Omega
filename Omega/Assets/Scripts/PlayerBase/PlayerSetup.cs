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
                playerIconID.playerDeadText.SetActive(true);
                Image iconBackground = playerIconID.iconBackground.GetComponent<Image>();
                iconBackground.color = new Color(iconBackground.color.r, iconBackground.color.g, iconBackground.color.b, iconBackground.color.a / 2);
                playerSpawnHandler.playerImageList.Remove(playerSpawnHandler.playerImageList[playerID - 1]);
            }
        }

        public void UpdatePlayerID()
        {
            playerID -= 1;
        }
    }
}