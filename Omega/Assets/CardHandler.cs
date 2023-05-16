using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    public class CardHandler : MonoBehaviour
    {
        public List<Card> cards;

        private List<Card> lowChanceCards;

        private List<Card> mediumChanceCards;

        private List<Card> highChanceCards;

        private void Awake()
        {
            foreach(Card card in cards)
            {
                if(card.cardOdds == Card.CardOdds.Low)
                {
                    lowChanceCards.Add(card);
                }
                if (card.cardOdds == Card.CardOdds.Medium)
                {
                    mediumChanceCards.Add(card);
                }
                if (card.cardOdds == Card.CardOdds.High)
                {
                    highChanceCards.Add(card);
                }
            }
        }

        public void DrawCard(GameObject player)
        {
            int ran = Random.Range(0, 100);

            if(ran > 83.33)
            {
                int rand = Random.Range(0, lowChanceCards.Count);
                player.GetComponent<PlayerCards>().cardsInHand.Add(lowChanceCards[rand]);
            }
            else if (ran < 83.33 && ran > 50)
            {
                int rand = Random.Range(0, mediumChanceCards.Count);
                player.GetComponent<PlayerCards>().cardsInHand.Add(mediumChanceCards[rand]);
            }
            else
            {
                int rand = Random.Range(0, highChanceCards.Count);
                player.GetComponent<PlayerCards>().cardsInHand.Add(highChanceCards[rand]);
            }
        }
    }
}