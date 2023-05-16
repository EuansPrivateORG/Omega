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

        private void Start()
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

        public void DrawCard()
        {
            int ran = Random.Range(1, 100);

            if(ran > 84)
            {
                
            }
        }
    }
}