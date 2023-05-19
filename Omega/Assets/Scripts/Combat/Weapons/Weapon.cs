using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Combat
{
    public class Weapon : MonoBehaviour
    {
        public weaponClass weaponType;

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
