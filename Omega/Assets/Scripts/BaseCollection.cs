using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Visual
{
    public class BaseCollection : MonoBehaviour
    {
        [SerializeField] public GameObject baseParent;
        [SerializeField] public GameObject destroyedParent;
        [SerializeField] public GameObject destroyedRigidParent;
        [SerializeField] public GameObject basePiecesParent;
        [SerializeField] public GameObject destroyedBasePiecesParent;
        [SerializeField] public GameObject emissivePiecesParent;
        [SerializeField] public GameObject iconParent;
        [SerializeField] public GameObject destroyedIconParent;
        [SerializeField] public List<GameObject> emissiveGunPartsList = new List<GameObject>();
        [SerializeField] public GameObject pipeParent;
        [SerializeField] public Material destroyedPipeMat;
    }
}
