using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Combat
{
    public class Weapon : MonoBehaviour
    {

        public weaponClass weaponType;
        public GameObject projectilePrefab;
        public float shakeOnShot;
        public float shakeOnImpact;
        public float shakOnShotLength;
        public float shakOnImpactLength;
        [System.Serializable]
        public enum weaponClass
        {
            Light,
            Medium,
            Heavy,
            Ultimate
        }
    }
}
