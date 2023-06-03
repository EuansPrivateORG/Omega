using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Visual
{
    public class BaseCollection : MonoBehaviour
    {
        [SerializeField] public GameObject basePiecesParent;
        [SerializeField] public GameObject emissivePiecesParent;
        [SerializeField] public GameObject iconParent;
        [SerializeField] public List<GameObject> emissiveGunPartsList = new List<GameObject>();
    }
}
