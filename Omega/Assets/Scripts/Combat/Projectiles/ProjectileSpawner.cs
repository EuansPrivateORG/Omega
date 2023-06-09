using Omega.Actions;
using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Omega.Combat
{
    public class ProjectileSpawner : MonoBehaviour
    {
        PlayerIdentifier playerIdentifier;

        [SerializeField] public GameObject UltimateVFX;

        [HideInInspector] public List<GameObject> playersToStopAttack;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }

        public void SpawnProjectile(int damage,GameObject attackweapon, GameObject target, int minColour, int maxColour, AttackButtonHandler attackButtonHandler, int num)
        {
            foreach(GameObject player in playersToStopAttack)
            {
                if (target == player || playerIdentifier.currentPlayer == player)
                {
                    attackButtonHandler.continueWithAttack = true;
                    if(num == 0)
                    {
                        Debug.Log("ending turn");
                        NumberRoller numberRoller = FindObjectOfType<NumberRoller>();
                        numberRoller.TurnOffNumberRoller();
                        CardHandler cardHandler = FindObjectOfType<CardHandler>();
                        cardHandler.StartCoroutine(cardHandler.DelayNextTurn());
                    }
                    return;
                }
            }

            //Add CameraShake on shot
            CameraShake cameraShake = playerIdentifier.currentPlayer.GetComponent<PlayerCam>().playerCam.GetComponent<CameraShake>();
            Weapon weapon1 = attackweapon.GetComponent<Weapon>();
            cameraShake.ShakeCamera(weapon1.shakeOnShot, weapon1.shakOnShotLength);
            Gamepad gamepad = Gamepad.current;
            if(weapon1.weaponType == Weapon.weaponClass.Light)
            {
                cameraShake.StartCoroutine(cameraShake.RumbleCoroutine(gamepad, cameraShake.intensity, 0.2f));
            }
            if(weapon1.weaponType == Weapon.weaponClass.Heavy)
            {
                cameraShake.StartCoroutine(cameraShake.RumbleCoroutine(gamepad, cameraShake.intensity*1.5f, 3f));
            }

            GameObject projectileToFire = null;
            if(attackButtonHandler.weaponClass != Weapon.weaponClass.Ultimate)
            {
                GameObject projectileInstance = Instantiate(weapon1.projectilePrefab, attackweapon.transform);
                attackweapon.GetComponent<AudioSource>().Play();
                projectileToFire = projectileInstance;
                if(attackButtonHandler.weaponClass == Weapon.weaponClass.Medium)
                {
                    projectileInstance.GetComponent<MediumAttack>().target = target;
                    projectileInstance.transform.parent.GetComponentInChildren<MediumAttack>().attackWeapon = attackweapon;
                }
            }
            else
            {
                GameObject targetUltimatePosition = null;
                foreach (Weapon _weapon in target.GetComponentsInChildren<Weapon>())
                {
                    if(_weapon.weaponType == Weapon.weaponClass.Ultimate)
                    {
                        targetUltimatePosition = _weapon.gameObject;
                    }
                }
                GameObject projectileInstance = Instantiate(attackweapon.GetComponent<Weapon>().projectilePrefab, targetUltimatePosition.transform);
                GameObject instantiated = Instantiate(UltimateVFX, target.transform.position, Quaternion.identity);
                Debug.Log(instantiated);
                attackweapon.GetComponent<AudioSource>().Play();
                projectileToFire = projectileInstance;
            }

            projectileToFire.transform.parent = transform;
            projectileToFire.GetComponentInChildren<Projectile>().SetTarget(target.gameObject, playerIdentifier.currentPlayer, damage, minColour, maxColour, attackButtonHandler, num, attackweapon);

            if (weapon1.weaponType == Weapon.weaponClass.Light)
            {
                playerIdentifier.currentAttack.continueWithAttack = true;
                foreach(GameObject vfx in weapon1.muzzleFlash)
                {
                    vfx.GetComponent<ParticleSystem>().Play();
                }
            }
        }
    }
}
