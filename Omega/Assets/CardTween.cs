using Autodesk.Fbx;
using Omega.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.UI
{
    public class CardTween : MonoBehaviour
    {
        [SerializeField] public GameObject card1;
        [SerializeField] public GameObject card2;
        [SerializeField] public GameObject card3;
        [SerializeField] public GameObject card4;
        [SerializeField] public GameObject card5;

        [SerializeField] Transform cardUpPos;
        [SerializeField] Vector3 cardUpScale;

        [Tooltip("Time it takes for the active card to move")]
        [SerializeField] float cardUpTime;

        [Tooltip("Time it takes for the cards to move along in the deck")]
        [SerializeField] float cardMoveTime;

        private GameObject upCard = null;
        private Vector3 originalCardScale;

        private Vector3 firstCardPos;
        private Vector3 secondCardPos;
        private Vector3 thirdCardPos;
        private Vector3 fourthCardPos;
        private Vector3 fifthCardPos;

        public List<GameObject> cards = new List<GameObject>();
        public List<GameObject> cardPositions = new List<GameObject>();

        private void Awake()
        {
            cardPositions.Add(card1);
            cardPositions.Add(card2);
            cardPositions.Add(card3);
            cardPositions.Add(card4);
            cardPositions.Add(card5);
        }

        private void Start()
        {
            firstCardPos = card1.transform.position;
            secondCardPos = card2.transform.position;
            thirdCardPos = card3.transform.position;
            fourthCardPos = card4.transform.position;
            fifthCardPos = card5.transform.position;
        }

        private void RefreshCardList()
        {
            foreach (GameObject cardPosition in cardPositions)
            {
                if(cardPosition.transform.childCount > 0)
                {
                    cards.Add(cardPosition.transform.GetChild(0).gameObject);
                }
            }
        }

        private void CardUp(GameObject card)
        {
            upCard = card;
            originalCardScale = card.transform.localScale;
            LeanTween.scale(card, cardUpScale, cardUpTime);
            LeanTween.move(card, cardUpPos, cardUpTime);
        }

        private void CardDown()
        {
            if(upCard != null)
            {
                LeanTween.scale(upCard, originalCardScale, cardUpTime);
                LeanTween.move(upCard, fifthCardPos, cardUpTime);

                MoveCardsAlong();
            }
        }

        private void MoveCardsAlong()
        {
            foreach(GameObject card in cards)
            {
                if(card.transform.parent.position == secondCardPos)
                {
                    LeanTween.move(card, firstCardPos, cardMoveTime);
                }

                else if (card.transform.parent.position == thirdCardPos)
                {
                    LeanTween.move(card, secondCardPos, cardMoveTime);
                }

                else if (card.transform.parent.position == fourthCardPos)
                {
                    LeanTween.move(card, thirdCardPos, cardMoveTime);
                }

                else if (card.transform.parent.position == fifthCardPos)
                {
                    LeanTween.move(card, fourthCardPos, cardMoveTime);
                }
            }
        }
    }
}