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
        [SerializeField] public GameObject emptyPreFab;
        [SerializeField] public List<GameObject> baseVarientPrefabList;
        [SerializeField] public Material materialVarientOverrite;
        [SerializeField] public GameObject turnOrderVarientIcon;
        [SerializeField] public GameObject bannerIcon;
    }
}
