using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Omega.Core
{
    public class PlayerSetup : MonoBehaviour
    {
        public Base playerBase;

        [HideInInspector] public GameObject playerPreFab;

        [HideInInspector] public int playerID = 0;

        void Start()
        {
            playerPreFab = playerBase.baseVarientPrefabList[Random.Range(0, playerBase.baseVarientPrefabList.Count)];
            GameObject instantiated = Instantiate(playerPreFab, transform);

            MeshRenderer playerMesh = instantiated.GetComponent<MeshRenderer>();
            playerMesh.material = playerBase.materialVarientOverrite;

            PlayerSpawnHandler playerSpawner = FindObjectOfType<PlayerSpawnHandler>();
            GameObject icon = playerSpawner.playerImageList[playerID - 1];
            icon.GetComponent<Image>().color = playerBase.materialVarientOverrite.color;
        }

    }

}