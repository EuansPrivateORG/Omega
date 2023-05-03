using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Omega.Core
{
    [CreateAssetMenu(fileName = "New BaseVarient", menuName = "Create New PlayerBase")]
    public class Base : ScriptableObject
    {
        [SerializeField] public string factionName;
        [SerializeField] List<GameObject> BaseVarientPrefabList;
        [SerializeField] public Material materialvarientOverrite;
        [SerializeField] public Image turnOrderVarientIcon;

        public GameObject GetBaseVarientPrefab()
        {
            return BaseVarientPrefabList[Random.Range(0, BaseVarientPrefabList.Count)];
        }

    }
}
