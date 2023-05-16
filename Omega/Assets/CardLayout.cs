using Omega.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.UI
{
    public class CardLayout : MonoBehaviour
    {
        public float cardSpacing = 1f;
        public float cardWidth = 2f;
        public float cardHeight = 3f;
        public List<GameObject> cards = new List<GameObject>();

        void Start()
        {
            LayoutCards();
        }

        void LayoutCards()
        {
            // Sort cards by category
            Dictionary<Card.CardCategory, List<GameObject>> cardCategories = new Dictionary<Card.CardCategory, List<GameObject>>();
            foreach (GameObject card in cards)
            {
                Card cardData = card.GetComponent<PlayerCards>().cardsInHand[1]; // Change this line
                if (!cardCategories.ContainsKey(cardData.cardCategory))
                {
                    cardCategories.Add(cardData.cardCategory, new List<GameObject>());
                }
                cardCategories[cardData.cardCategory].Add(card);
            }

            // Determine the number of categories and the total width of each category
            int numCategories = cardCategories.Count;
            float totalWidth = numCategories * cardWidth;
            foreach (Card.CardCategory category in cardCategories.Keys)
            {
                totalWidth += (cardCategories[category].Count - 1) * cardSpacing;
            }

            // Determine the starting x position for the layout
            float startX = -(totalWidth - cardWidth) / 2f;

            // Lay out the cards
            foreach (Card.CardCategory category in cardCategories.Keys)
            {
                float xPos = startX + (cardWidth + cardSpacing) * (int)category;
                float startY = cardHeight / 2f;
                float yPos = startY;
                foreach (GameObject card in cardCategories[category])
                {
                    Vector3 cardPos = new Vector3(xPos, yPos, 0f);
                    card.transform.position = cardPos;
                    yPos -= cardHeight;
                }
            }

            MoveCardsCloser();
        }

        void MoveCardsCloser()
        {
            float currentSpacing = Vector3.Distance(cards[0].transform.position, cards[1].transform.position);

            if (currentSpacing > cardSpacing)
            {
                float newSpacing = currentSpacing - cardSpacing;

                for (int i = 0; i < cards.Count - 1; i++)
                {
                    Vector3 currentPos = cards[i].transform.position;
                    Vector3 nextPos = cards[i + 1].transform.position;
                    Vector3 direction = (nextPos - currentPos).normalized;
                    Vector3 newPos = currentPos + direction * newSpacing;
                    cards[i + 1].transform.position = newPos;
                }

                MoveCardsCloser();
            }
        }

        //public void ExpandCategory(Card.CardCategory category)
        //{
        //    // Find all cards in the given category
        //    List<Card> cardsInCategory = new List<Card>();
        //    foreach (Card card in cards)
        //    {
        //        if (card.cardCategory == category)
        //        {
        //            cardsInCategory.Add(card);
        //        }
        //    }

        //    // Determine the width of each card and the desired spacing between cards
        //    float cardWidth = cardPrefab.GetComponent<RectTransform>().sizeDelta.x;
        //    float spacing = cardWidth * cardSpacing;

        //    // Calculate the total width of the expanded group
        //    float totalWidth = (cardsInCategory.Count - 1) * spacing + cardWidth;

        //    // Determine the starting position for the group
        //    Vector3 startPos = transform.position - new Vector3(totalWidth / 2f, 0f, 0f);

        //    // Set the positions of the cards in the group
        //    for (int i = 0; i < cards.Count; i++)
        //    {
        //        Card card = cards[i];
        //        if (card.cardCategory == category)
        //        {
        //            float x = startPos.x + i * spacing;
        //            Vector3 pos = new Vector3(x, transform.position.y, transform.position.z);
        //            card.gameObject.transform.position = pos;
        //        }
        //    }
        //}

    }
}
