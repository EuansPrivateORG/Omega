using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Core
{
    [CreateAssetMenu(fileName = "New BaseVarient", menuName = "Create New PlayerBase")]
    public class Base : ScriptableObject
    {
        [SerializeField] GameObject BaseVarientPrefab;
        [SerializeField] GameObject TurnOrderVarientPrefab;

    }
}
