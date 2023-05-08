using Omega.Core;
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

        public void SpawnProjectile(int damage, Transform target)
        {
            GameObject instantiatedProjectile = Instantiate(projectile, playerIdentifier.currentPlayer.gameObject.transform);
            instantiatedProjectile.GetComponent<Projectile>().SetDamage(damage);
            instantiatedProjectile.GetComponent<Rigidbody>().AddForce(10, 10, 10);
            instantiatedProjectile.transform.LookAt(target);
        }
    }
}
