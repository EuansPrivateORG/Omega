using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;
using Omega.UI;
using Unity.Mathematics;

namespace Omega.Core
{

    public class CardSpawner : MonoBehaviour
    {
        private List<Card> cardsToSpawn = new List<Card>();

        private CardTween cardTween;

        private List<GameObject> activeCards = new List<GameObject>();

        private void Awake()
        {
            cardTween = FindObjectOfType<CardTween>();
        }

        public void SpawnCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                cardsToSpawn.Add(card);
            }

            for(int i = 0; i < cardsToSpawn.Count; i++)
            { 
                GameObject instantiated = Instantiate(cardsToSpawn[i].CardUIPreFab, cardTween.cardPositions[i].transform);
                activeCards.Add(instantiated);
                instantiated.transform.parent.GetComponent<CardButtonHandler>().currentCardHighlight = instantiated.GetComponent<CardCollection>().cardHighlight;
            }

            cardsToSpawn.Clear();
        }

        public void AddCard(Card card)
        {
            bool hasBeenGiven = false;
            for(int i = 0; i < cardTween.cardPositions.Count; i++)
            {
                if (!hasBeenGiven)
                {
                    if (i == activeCards.Count)
                    {
                        GameObject instantiated = Instantiate(card.CardUIPreFab, cardTween.cardPositions[i].transform);
                        activeCards.Add(instantiated);
                        hasBeenGiven = true;
                    }
                }
            }
        }

        public void ClearActiveCards()
        {
            foreach(GameObject instantiated in activeCards)
            {
                instantiated.transform.parent.GetComponent<CardButtonHandler>().currentCardHighlight = null;
                instantiated.transform.parent.DetachChildren();
                Destroy(instantiated);
            }

            activeCards.Clear();
        }

        public void RemoveCard(GameObject cardToRemove)
        {
            activeCards.Remove(cardToRemove);

            Destroy(cardToRemove);
        }
    }
}