using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Core;

namespace Omega.Status
{
    public class Health : MonoBehaviour
    {
        [HideInInspector] public int currentHealth;

        [HideInInspector] public int maxHealth;

        [HideInInspector] public bool isDead = false;

        public void AddHealth(int addition)
        {
            currentHealth += addition;
        }

        public void TakeDamage(int loss)
        {
            currentHealth -= loss;

            if(currentHealth <= 0)
            {
                isDead = true;
                PlayerSetup playerSetup = GetComponent<PlayerSetup>();
                playerSetup.SetPlayerIconDead();

                int playerID = playerSetup.playerID;
                foreach(GameObject player in FindObjectOfType<PlayerIdentifier>().currentlyAlivePlayers)
                {
                    if(player.GetComponent<PlayerSetup>().playerID > playerID)
                    {
                        player.GetComponent<PlayerSetup>().UpdatePlayerID();
                    }
                }
            }
        }

        public void SetHealth()
        {
            currentHealth = maxHealth;
        }
    }
}