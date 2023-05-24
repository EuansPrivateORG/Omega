using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.UI
{
    public class PlayerIconID : MonoBehaviour
    {
        public GameObject playerIcon;
        public GameObject iconBackground;
        public GameObject playerDeadText;
        [SerializeField] public float moveDownAmountY;
        [HideInInspector]
        public float yAxisMovement;

        private void Awake()
        {
            yAxisMovement = transform.position.y - moveDownAmountY;
        }


    }
}