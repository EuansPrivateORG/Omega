using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Actions
{
    public class PlayerCards : MonoBehaviour
    {
        public List<Card> cardsInHand = new List<Card>();
        public List<Card> cardsPlayed = new List<Card>();
        public List<Card> cardsPlayedAgainst = new List<Card>();

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
    }
}