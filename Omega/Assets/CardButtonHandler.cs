using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Omega.UI
{
    public class CardButtonHandler : MonoBehaviour
    {
        EventSystem eventSystem;

        [HideInInspector] public GameObject currentCardHighlight = null;

        private CardTween cardTween;

        private bool cardSelected = false;
        private bool hasUnSelectedCard = false;


        private void Awake()
        {
            eventSystem = EventSystem.current;
            cardTween = FindObjectOfType<CardTween>();
        }

        void Update()
        {
            if (eventSystem.currentSelectedGameObject == gameObject)
            {
                if(!cardSelected)
                {
                    SelectCard();
                    cardSelected = true;
                    hasUnSelectedCard = false;
                }

                if(currentCardHighlight != null)
                {
                    currentCardHighlight.SetActive(true);
                }
            }
            else
            {
                if(cardSelected && !hasUnSelectedCard)
                {
                    cardSelected = false;
                    hasUnSelectedCard = true;
                    UnSelectCard();
                }

                if (currentCardHighlight != null)
                {
                    currentCardHighlight.SetActive(false);
                }
            }
        }

        public void SelectCard()
        {
            Debug.Log("selected card = " + gameObject);
            cardTween.CardUp(gameObject);
        }

        public void UnSelectCard()
        {
            Debug.Log("unselected card = " + gameObject);
            cardTween.CardDown(gameObject);
        }
    }
}