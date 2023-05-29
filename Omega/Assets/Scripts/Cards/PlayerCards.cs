using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Omega.Actions
{
    public class PlayerCards : MonoBehaviour
    {
        public List<Card> cardsInHand = new List<Card>();
        public List<Card> cardsPlayed = new List<Card>();
        public List<Card> cardsPlayedAgainst = new List<Card>();

        public List<Transform> cardDeckPositions;

        public List<GameObject> playingCardInDeck = new List<GameObject>();

        public List<Transform> playingCardPositions;

        public List<GameObject> playedCardInWorld = new List<GameObject>();

        public TextMeshProUGUI cardNumText;


        private CardHandler cardHandler;

        private void Awake()
        {
            cardHandler = FindObjectOfType<CardHandler>();
        }

        void Start()
        {

        }

        public void StartingCards()
        {
            cardHandler.DrawCardNoUI(gameObject, 2);
        }

        void Update()
        {

        }

        public void InstantiatedCardInDeck(GameObject card)
        {
            Transform cardSpawnPos = null;

            for (int i = 0; i < cardDeckPositions.Count; i++)
            {
                if (cardDeckPositions[i].transform.childCount > 0) continue;
                else
                {
                    cardSpawnPos = cardDeckPositions[i];
                    break;
                }
            }

            GameObject instantiated = Instantiate(card, cardSpawnPos);

            playingCardInDeck.Add(instantiated);

            instantiated.name = card.name;

            StartCoroutine(ReappearCard(card));

            cardNumText.text = cardsInHand.Count.ToString();
        }

        public void InstantiatePlayedCards(GameObject card)
        {
            RemoveCardFromDeck(card);

            Transform spawnPos = null;
            for (int i = 0; i < playingCardPositions.Count; i++)
            {
                if (playingCardPositions[i].childCount <= 0)
                {
                    spawnPos = playingCardPositions[i];
                    break;
                }
            }

            GameObject instantiated = Instantiate(card, spawnPos);
            playedCardInWorld.Add(instantiated);
            instantiated.name = card.name;
            StartCoroutine(ReappearCard(card));
        }

        public void RemoveCardFromDeck(GameObject card)
        {
            GameObject cardInDeck = null;

            foreach (GameObject cards in playingCardInDeck)
            {
                if (cards.name == card.name && cards != card)
                {
                    cardInDeck = cards;
                    break;
                }
            }

            if(cardInDeck != null)
            {
                playingCardInDeck.Remove(cardInDeck);
                StartCoroutine(DissolveCard(cardInDeck, true));
            }

            cardNumText.text = cardsInHand.Count.ToString();
        }

        public void RemovePlayedCards(GameObject card)
        {
            GameObject playedCard = null;

            foreach (GameObject cards in playedCardInWorld)
            {
                if (cards.name == card.name && cards != card)
                {
                    playedCard = cards;
                    break;
                }
            }


            if (playedCard != null)
            {
                playedCardInWorld.Remove(playedCard);
                StartCoroutine(DissolveCard(playedCard, false));
            }
        }

        private IEnumerator DissolveCard(GameObject card, bool fromDeck)
        {
            Renderer cardRenderer = card.GetComponent<Renderer>();
            CanvasGroup cardCanvas = card.GetComponent<CardCollection3D>().cardToUse.GetComponent<CanvasGroup>();
            float dissolveDuration = Random.Range(1f, 2f);
            float elapsedTime = 0f;

            while (elapsedTime < dissolveDuration)
            {
                float tRenderer = elapsedTime / dissolveDuration;
                float tCanvas = 0.5f - (elapsedTime / dissolveDuration);

                cardRenderer.material.SetFloat("_Dissolve", tRenderer);
                cardCanvas.alpha = tCanvas;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cardRenderer.material.SetFloat("_Dissolve", 1f);
            cardCanvas.alpha = 0f;

            if (fromDeck)
            {
                StartCoroutine(MoveCardsDown(card));
            }
            else
            {
                Destroy(card);
            }
        }

        private IEnumerator ReappearCard(GameObject card)
        {
            //Debug.Log(card.name);
            Renderer cardRenderer = card.GetComponent<Renderer>();
            CanvasGroup cardCanvas = card.GetComponent<CardCollection3D>().cardToUse.GetComponent<CanvasGroup>();
            float dissolveDuration = Random.Range(1f, 2f);
            cardCanvas.alpha = 0f;
            float elapsedTime = 0f;

            while (elapsedTime < dissolveDuration)
            {
                float tCanvas = elapsedTime / dissolveDuration;
                float tRenderer = 1f - (elapsedTime / dissolveDuration);

                cardRenderer.sharedMaterial.SetFloat("_Dissolve", tRenderer);
                cardCanvas.alpha = tCanvas;
                //Debug.Log(gameObject.name + " " + cardCanvas + " " + cardCanvas.alpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cardRenderer.sharedMaterial.SetFloat("_Dissolve", 0f);
            cardCanvas.alpha = 1f;
        }

        private IEnumerator MoveCardsDown(GameObject playedCard)
        {
            int cardPosLost = 0;
            float lerpDuration = 0.2f;

            for (int i = 0; i < cardDeckPositions.Count; i++)
            {
                if (cardDeckPositions[i] == playedCard.transform.parent)
                {
                    cardPosLost = i;
                    break;
                }
            }

            for (int i = cardPosLost + 1; i < cardDeckPositions.Count; i++)
            {
                Transform currentPosition = cardDeckPositions[i];
                Transform nextPosition = cardDeckPositions[i - 1];

                if (currentPosition.childCount > 0)
                {
                    Transform cardPosChild = currentPosition.GetChild(0);

                    Vector3 startPos = cardPosChild.position;
                    Vector3 endPos = nextPosition.position;

                    float elapsedTime = 0f;
                    while (elapsedTime < lerpDuration)
                    {
                        float t = elapsedTime / lerpDuration;
                        cardPosChild.position = Vector3.Lerp(startPos, endPos, t);

                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    cardPosChild.SetParent(nextPosition);
                    cardPosChild.localPosition = Vector3.zero;
                }
            }

            Destroy(playedCard);
        }
    }
}