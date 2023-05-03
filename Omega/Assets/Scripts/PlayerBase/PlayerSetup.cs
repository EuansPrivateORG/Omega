using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Core
{
    public class PlayerSetup : MonoBehaviour
    {
        public Base playerBase;

        [HideInInspector] public GameObject playerPreFab;

        void Start()
        {
            playerPreFab = playerBase.baseVarientPrefabList[Random.Range(0, playerBase.baseVarientPrefabList.Count)];
            GameObject instantiated = Instantiate(playerPreFab, transform);

            MeshRenderer playerMesh = instantiated.GetComponent<MeshRenderer>();
            playerMesh.material = playerBase.materialVarientOverrite;
        }

    }

}