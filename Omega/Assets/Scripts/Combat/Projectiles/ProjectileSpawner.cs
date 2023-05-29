using Omega.Actions;
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
        PlayerIdentifier playerIdentifier;


        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }

        public void SpawnProjectile(int damage,GameObject attackweapon, GameObject target, int minColour, int maxColour, AttackButtonHandler attackButtonHandler, int num)
        {
            //Add CameraShake on shot
            CameraShake cameraShake = playerIdentifier.currentPlayer.GetComponent<PlayerCam>().playerCam.GetComponent<CameraShake>();
            Weapon weapon1 = attackweapon.GetComponent<Weapon>();
            cameraShake.ShakeCamera(weapon1.shakeOnShot, weapon1.shakOnShotLength);

            GameObject projectileToFire = null;
            if(attackButtonHandler.weaponClass != Weapon.weaponClass.Ultimate)
            {
                GameObject projectileInstance = Instantiate(attackweapon.GetComponent<Weapon>().projectilePrefab, attackweapon.transform);
                attackweapon.GetComponent<AudioSource>().Play();
                projectileToFire = projectileInstance;
            }
            else
            {
                GameObject targetUltimatePosition = null;
                foreach (Weapon weapon in target.GetComponentsInChildren<Weapon>())
                {
                    if(weapon.weaponType == Weapon.weaponClass.Ultimate)
                    {
                        targetUltimatePosition = weapon.gameObject;
                    }
                }
                GameObject projectileInstance = Instantiate(attackweapon.GetComponent<Weapon>().projectilePrefab, attackweapon.transform);
                attackweapon.GetComponent<AudioSource>().Play();
                projectileToFire = projectileInstance;
            }

            projectileToFire.transform.parent = transform;
            projectileToFire.GetComponentInChildren<Projectile>().SetTarget(target.gameObject, playerIdentifier.currentPlayer, damage, minColour, maxColour, attackButtonHandler, num, attackweapon);
        }
    }
}
