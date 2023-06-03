
using Omega.Actions;
using Omega.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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

        public AudioSource cardTransitionSource;

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

        private CardHandler cardHandler;

        private CardSpawner cardSpawner;

        private DrawCardHandler drawCardHandler;

        private void Awake()
        {
            cardHandler = FindObjectOfType<CardHandler>();
            cardSpawner = FindObjectOfType<CardSpawner>();
            drawCardHandler = FindObjectOfType<DrawCardHandler>();

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
                }
            }

            if (card1.transform.childCount > 0)
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = drawCardHandler.healingButton;
                newNav.selectOnLeft = card1.GetComponent<Button>();
                drawCardHandler.attackButton.navigation = newNav;
            }
            else
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = drawCardHandler.healingButton;
                drawCardHandler.attackButton.navigation = newNav;
            }
        }

        public void CardUp(GameObject card)
        {
            upCard = card;
            originalCardScale = card.transform.localScale;
            LeanTween.scale(card, cardUpScale, cardUpTime);
            LeanTween.move(card, cardUpPos, cardUpTime);
            cardTransitionSource.Play();
        }


        public void CardDown(GameObject card)
        {
            GameObject upCardTarget = null;
            LeanTween.scale(card, originalCardScale, cardUpTime);
            for (int i = 0; i < cards.Count; i++)
            {
                if(i == cards.Count - 1)
                {
                    upCardTarget = cardPositions[i];
                    LeanTween.move(card, cardPositions[i].transform.position, cardUpTime);
                    card.transform.SetParent(cardPositions[i].transform);
                    if (cardPositions[i] == card1)
                    {
                        cardPositions[i].GetComponent<CardButtonHandler>().currentCardHighlight.SetActive(false);
                        cardPositions[i].GetComponent<CardButtonHandler>().currentCardHighlight = card.GetComponent<CardCollection>().cardHighlight;
                    }
                }
            }

            upCard = card;

            MoveCardsAlong(upCardTarget);
        }

        public void CardPlayed(GameObject card)
        {
            CardDown(card);
            cards.Remove(card);
            cardHandler.CardPlayed(card);
            cardSpawner.RemoveCard(card);

            if(cards.Count <= 0)
            {
                Navigation newNav = new Navigation();
                newNav.mode = Navigation.Mode.Explicit;
                newNav.selectOnRight = drawCardHandler.healingButton;
                drawCardHandler.attackButton.navigation = newNav;

                EventSystem.current.SetSelectedGameObject(drawCardHandler.attackButton.gameObject);
            }
            else
            {
                CardUp(card1.transform.GetChild(0).gameObject);
            }
        }


        private void MoveCardsAlong(GameObject upCardTarget)
        {
            GameObject cardInFirstPos = null;
            GameObject cardInSecondPos = null;
            GameObject cardInThirdPos = null;
            GameObject cardInFourthPos = null;
            GameObject placedCard = null;

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] == upCard)
                {
                    placedCard = cards[i];
                    placedCard.transform.SetParent(upCardTarget.transform);
                    if(upCardTarget == card1)
                    {
                        upCardTarget.GetComponent<CardButtonHandler>().currentCardHighlight.SetActive(false);
                        upCardTarget.GetComponent<CardButtonHandler>().currentCardHighlight = placedCard.GetComponent<CardCollection>().cardHighlight;
                    }
                }

                else if (cards[i].transform.parent.position == secondCardPos)
                {
                    cardInFirstPos = cards[i];
                    LeanTween.move(cardInFirstPos, card1.transform.position, cardMoveTime);
                    cardInFirstPos.transform.SetParent(card1.transform);
                    card1.GetComponent<CardButtonHandler>().currentCardHighlight.SetActive(false);
                    card1.GetComponent<CardButtonHandler>().currentCardHighlight = cards[i].GetComponent<CardCollection>().cardHighlight;
                }

                else if (cards[i].transform.parent.position == thirdCardPos)
                {
                    cardInSecondPos = cards[i];
                    LeanTween.move(cardInSecondPos, card2.transform.position, cardMoveTime);
                    cardInSecondPos.transform.SetParent(card2.transform);
                }

                else if (cards[i].transform.parent.position == fourthCardPos)
                {
                    cardInThirdPos = cards[i];
                    LeanTween.move(cardInThirdPos, card3.transform.position, cardMoveTime);
                    cardInThirdPos.transform.SetParent(card3.transform);
                }

                else if (cards[i].transform.parent.position == fifthCardPos)
                {
                    cardInFourthPos = cards[i];
                    LeanTween.move(cardInFourthPos, card4.transform.position, cardMoveTime);
                    cardInFourthPos.transform.SetParent(card4.transform);
                }
            }
        }
    }
}