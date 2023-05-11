using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Combat
{
    public class ProjectileSpawner : MonoBehaviour
    {
        [SerializeField] GameObject projectile;
        PlayerIdentifier playerIdentifier;


        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }

        public void SpawnProjectile(int damage, GameObject target, int minColour, int maxColour, AttackButtonHandler attackButtonHandler)
        {
            
            GameObject projectileInstance = Instantiate(projectile, playerIdentifier.currentPlayer.transform.position, Quaternion.identity);
            projectileInstance.transform.parent = transform;
            projectileInstance.GetComponentInChildren<Projectile>().SetTarget(target.gameObject, playerIdentifier.currentPlayer, damage, minColour, maxColour, attackButtonHandler);
        }
    }
}