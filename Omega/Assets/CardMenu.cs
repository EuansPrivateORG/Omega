using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Omega.UI
{
    public class CardMenu : MonoBehaviour
    {
        public float upScaleAmount = 1.5f;
        public float upAmount = 10f;
        public float movingTime = 0.5f;
        private Vector3 upScale;
        private Vector3 downScale;
        private EventSystem eventSystem;
        private bool isUp = false;
        private bool isDown = true;
        private float originalY;
        private int siblingIndex;

        private void Start()
        {
            downScale = transform.localScale;
            upScale = downScale * upScaleAmount;
            eventSystem = EventSystem.current;
            originalY = transform.position.y;
            siblingIndex = transform.GetSiblingIndex();
        }

        void Update()
        {
            if(eventSystem.currentSelectedGameObject == gameObject && !isUp)
            {
                isUp = true;
                isDown = false;

                LeanTween.scale(gameObject, upScale, movingTime);
                LeanTween.moveY(gameObject, originalY + upAmount, movingTime);
                transform.SetAsLastSibling();
            }
            else if(eventSystem.currentSelectedGameObject != gameObject && !isDown)
            {
                isDown = true;
                isUp = false;

                LeanTween.scale(gameObject, downScale, movingTime);
                LeanTween.moveY(gameObject, originalY, movingTime);
                transform.SetSiblingIndex(siblingIndex);
            }
        }
    }
}