using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.UI
{
    public class HealingSFXCollection : MonoBehaviour
    {
        [SerializeField] public AudioSource healingSource;

        private void Awake()
        {
            healingSource = GetComponent<AudioSource>();
        }
    }

   

}