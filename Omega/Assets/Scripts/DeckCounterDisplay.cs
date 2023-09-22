using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Omega.Core;

namespace Omega.Visual
{
    public class DeckCounterDisplay : MonoBehaviour
    {
        public List<Image> rotatingIcons = new List<Image>();


        private PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }
    }
}
