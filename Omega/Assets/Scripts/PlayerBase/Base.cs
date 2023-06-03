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
        [SerializeField] public Material baseMaterialVarientOverrite;
        [SerializeField] public Material emissiveMaterialVarientOverrite;
        [SerializeField] public GameObject turnOrderVarientIcon;
        [SerializeField] public GameObject faction3DIcon;
        [SerializeField] public Color uiOverriteColor;
       
    }
}
