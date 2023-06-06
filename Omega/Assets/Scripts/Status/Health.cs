using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Core;
using Omega.Visual;

namespace Omega.Status
{
    public class Health : MonoBehaviour
    {
        public GameObject cardCountDisplay;

        [HideInInspector] public int currentHealth;

        [HideInInspector] public int maxHealth;

        [HideInInspector] public bool isDead = false;

        private PlayerIdentifier playerIdenti;

        private ScoreHandler scoreHandler;

        private void Awake()
        {
            playerIdenti = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();

        }

        public void AddHealth(int addition)
        {
            currentHealth += addition;
            GetComponent<DamageStateCollection>().CheckHealth();
        }

        public void TakeDamage(int loss)
        {
            currentHealth -= loss;

            GetComponent<DamageStateCollection>().CheckHealth();

            if (currentHealth <= 0)
            {
                isDead = true;
                PlayerSetup playerSetup = GetComponent<PlayerSetup>();
                playerSetup.SetPlayerIconDead();

                int placement = playerIdenti.playerIndex.Count - (playerIdenti.currentlyAlivePlayers.Count - 1);
                
                int playerID = playerSetup.playerID - 1;

                scoreHandler.CalculatePlayerPlacement(placement, playerID);

                foreach(GameObject player in playerIdenti.currentlyAlivePlayers)
                {
                    if(player.GetComponent<PlayerSetup>().playerID > playerID)
                    {
                        player.GetComponent<PlayerSetup>().UpdatePlayerID();
                    }
                }

                DestroyBase();
            }
        }

        public void DestroyBase()
        {
            BaseCollection baseCollection = GetComponentInChildren<BaseCollection>();

            baseCollection.baseParent.SetActive(false);
            baseCollection.destroyedParent.SetActive(true);

            foreach(Transform child in baseCollection.destroyedRigidParent.transform)
            {
                Rigidbody childRigid = child.GetComponent<Rigidbody>();
                childRigid.AddForce(50f, 50f, 50f, ForceMode.Impulse);
            }

            foreach (Transform child in baseCollection.pipeParent.transform)
            {
                if(child.TryGetComponent<Renderer>(out Renderer childRen))
                {
                    childRen.material = baseCollection.destroyedPipeMat;
                }
            }

            cardCountDisplay.SetActive(false);

            GetComponent<BaseVFX>().PlayExplosion();
        }

        public void SetHealth()
        {
            currentHealth = maxHealth;
            GetComponent<DamageStateCollection>().CheckHealth();
        }
    }
}