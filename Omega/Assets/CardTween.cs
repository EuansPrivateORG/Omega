
using Omega.Actions;
using Omega.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


        private List<Vector3> cardOriginPositions = new List<Vector3>();
        public List<GameObject> cards = new List<GameObject>();
        public List<GameObject> cardPositions = new List<GameObject>();

        private DrawCardHandler drawCardHandler;

        private PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            drawCardHandler = FindObjectOfType<DrawCardHandler>();

            playerIdentifier = FindObjectOfType<PlayerIdentifier>();

            cardPositions.Add(card1);
            cardPositions.Add(card2);
            cardPositions.Add(card3);
            cardPositions.Add(card4);
            cardPositions.Add(card5);

            originalCardScale = card2.transform.localScale;
        }

        private void Start()
        {
            firstCardPos = card1.transform.position;
            secondCardPos = card2.transform.position;
            thirdCardPos = card3.transform.position;
            fourthCardPos = card4.transform.position;
            fifthCardPos = card5.transform.position;

            cardOriginPositions.Add(firstCardPos);
            cardOriginPositions.Add(secondCardPos);
            cardOriginPositions.Add(thirdCardPos);
            cardOriginPositions.Add(fourthCardPos);
            cardOriginPositions.Add(fifthCardPos);
        }

        public void RefreshCardList()
        {
            cards.Clear();

            foreach (GameObject cardPosition in cardPositions)
            {
                if(cardPosition.transform.childCount > 0)
                {
                    cards.Add(cardPosition.transform.GetChild(0).gameObject);
                    Debug.Log(cardPosition.transform.GetChild(0).gameObject);
                    Debug.Log(cardPosition);
                }
            }

            for (int i = 0; i < cardPositions.Count; i++)
            {
                cardPositions[i].transform.position = cardOriginPositions[i];
                if (cardPositions[i].transform.localScale != originalCardScale)
                {
                    LeanTween.scale(cardPositions[i], originalCardScale, cardMoveTime);
                }
            }


        }

        public void CardUp(GameObject card)
        {
            upCard = card;
            originalCardScale = card.transform.localScale;
            LeanTween.scale(card, cardUpScale, cardUpTime);
            LeanTween.move(card, cardUpPos, cardUpTime);
        }


        public void CardDown(GameObject card)
        {
            LeanTween.scale(card, originalCardScale, cardUpTime);

            for(int i = 0; i < cards.Count; i++)
            {
                if(i == cards.Count - 1)
                {
                    Debug.Log(i);
                    LeanTween.move(card, cardOriginPositions[i], cardUpTime);
                }
            }

            upCard = card.transform.GetChild(0).gameObject;

            MoveCardsAlong();
        }

        private void MoveCardsAlong()
        {
            GameObject cardInFirstPos = null;
            GameObject cardInSecondPos = null;
            GameObject cardInThirdPos = null;
            GameObject cardInFourthPos = null;
            GameObject placedCard = null;

            for (int i = 0; i < cards.Count; i++)
            {
                Debug.Log(cards.Count);

                if (cards[i] == upCard)
                {
                    placedCard = cardPositions[i];
                    cardPositions[i].transform.SetSiblingIndex(0);
                }

                else if (cards[i].transform.parent.position == secondCardPos)
                {
                    cardInFirstPos = cardPositions[i];
                    LeanTween.move(cardPositions[i], firstCardPos, cardMoveTime);
                    cardPositions[i].transform.SetSiblingIndex(5);
                }

                else if (cards[i].transform.parent.position == thirdCardPos)
                {
                    cardInSecondPos = cardPositions[i];
                    LeanTween.move(cardPositions[i], secondCardPos, cardMoveTime);
                    cardPositions[i].transform.SetSiblingIndex(4);
                }

                else if (cards[i].transform.parent.position == fourthCardPos)
                {
                    cardInThirdPos = cardPositions[i];
                    LeanTween.move(cardPositions[i], thirdCardPos, cardMoveTime);
                    cardPositions[i].transform.SetSiblingIndex(3);
                }

                else if (cards[i].transform.parent.position == fifthCardPos)
                {
                    cardInFourthPos = cardPositions[i];
                    LeanTween.move(cardPositions[i], fourthCardPos, cardMoveTime);
                    cardPositions[i].transform.SetSiblingIndex(2);
                }
            }

            SetNavigation(cardInFirstPos, cardInSecondPos, cardInThirdPos, cardInFourthPos, placedCard);
        }

        private void SetNavigation(GameObject cardInFirstPos, GameObject cardInSecondPos, GameObject cardInThirdPos, GameObject cardInFourthPos, GameObject placedCard)
        {
            Button cardInFirstPosButton = null;
            Button cardInSecondPosButton = null;
            Button cardInThirdPosButtton = null;
            Button cardInFourthPosButton = null;
            Button placedCardButton = null;

            if(cardInFirstPos != null)
            {
                cardInFirstPosButton = cardInFirstPos.GetComponent<Button>();
            }
            if(cardInSecondPos != null)
            {
                cardInSecondPosButton = cardInSecondPos.GetComponent<Button>();
            }
            if(cardInThirdPos != null)
            {
                cardInThirdPosButtton = cardInThirdPos.GetComponent<Button>();
            }
            if(cardInFourthPos != null)
            {
                cardInFourthPosButton = cardInFourthPos.GetComponent<Button>();
            }
            if(placedCard != null)
            {
                placedCardButton = placedCard.GetComponent<Button>();
            }

            if(cardInFirstPosButton != null)
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = drawCardHandler.healingButton;
                newNav.selectOnLeft = cardInFirstPosButton;
                drawCardHandler.attackButton.navigation = newNav;

                Navigation newNav2 = new Navigation();
                newNav2.mode = Navigation.Mode.Explicit;
                newNav2.selectOnRight = drawCardHandler.attackButton;
                if(cardInSecondPosButton != null) newNav2.selectOnLeft = cardInSecondPosButton;
                else
                {
                    newNav2.selectOnLeft = placedCardButton;

                    Navigation newNav3 = new Navigation();
                    newNav3.mode = Navigation.Mode.Explicit;
                    newNav3.selectOnRight = cardInFirstPosButton;
                    placedCardButton.navigation = newNav3;
                }
                cardInFirstPosButton.navigation = newNav2;
            }

            else if(cardInSecondPosButton != null)
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = cardInFirstPosButton;
                if (cardInThirdPosButtton != null) newNav.selectOnLeft = cardInThirdPosButtton;
                else
                {
                    newNav.selectOnLeft = placedCardButton;

                    Navigation newNav2 = new Navigation();
                    newNav2.mode = Navigation.Mode.Explicit;
                    newNav2.selectOnRight = cardInSecondPosButton;
                    placedCardButton.navigation = newNav2;
                }
                cardInSecondPosButton.navigation = newNav;
            }

            else if(cardInThirdPosButtton != null)
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = cardInSecondPosButton;
                if (cardInFourthPosButton != null) newNav.selectOnLeft = cardInFourthPosButton;
                else
                {
                    newNav.selectOnLeft = placedCardButton;

                    Navigation newNav2 = new Navigation();
                    newNav2.mode = Navigation.Mode.Explicit;
                    newNav2.selectOnRight = cardInThirdPosButtton;
                    placedCardButton.navigation = newNav2;
                }
                cardInSecondPosButton.navigation = newNav;
            }

            else if (cardInFourthPosButton != null)
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = cardInThirdPosButtton;
                newNav.selectOnLeft = placedCardButton;
                cardInSecondPosButton.navigation = newNav;

                Navigation newNav2 = new Navigation();
                newNav2.mode = Navigation.Mode.Explicit;
                newNav2.selectOnRight = cardInFourthPosButton;
                placedCardButton.navigation = newNav2;
            }
        }
    }
}