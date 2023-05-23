using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    public class CardCollection3D : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField] public GameObject cardToUse = null;

        private void Awake()
        {
            cardToUse = GetComponentInChildren<CardCollection>().gameObject;
        }
    }

}