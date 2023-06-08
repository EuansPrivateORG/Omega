using Cinemachine;
using Omega.Actions;
using Omega.Core;
using Omega.Status;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Omega.Combat
{
    public class Projectile : MonoBehaviour
    {
        public int damage;

        public float projectileLifetime = 3f;
        public float projectileSpeed = 10f;
        public AudioClip ImpactClip;

        GameObject instigator = null;
        public GameObject target = null;
        GameObject attackWeapon = null;

        int minColour;
        int maxColour;

        PlayerIdentifier playerIdentifier;
        AttackButtonHandler attackButtonHandler;
        ScoreHandler scoreHandler;
        NumberRoller numberRoller;
        CardHandler cardHandler;

        private int bulletNum;

        private bool playerHasDied = false;

        private bool hasAttacked = false;

        private float maxDistanceThreshold = 20f;


        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            scoreHandler = FindObjectOfType<ScoreHandler>();
            numberRoller = FindObjectOfType<NumberRoller>();
            cardHandler = FindObjectOfType<CardHandler>();
        }
        private void Update()
        {
            if (target == null) return;

            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * projectileSpeed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider enemyCollider)
        {
            if (!hasAttacked)
            {
                if (enemyCollider != instigator.GetComponent<Collider>())
                {
                    //Add CameraShake on impact
                    CameraShake cameraShake = instigator.GetComponent<PlayerCam>().playerCam.GetComponent<CameraShake>();
                    Weapon weapon = attackWeapon.GetComponent<Weapon>();
                    cameraShake.ShakeCamera(weapon.shakeOnImpact, weapon.shakOnImpactLength);

                    target.GetComponent<Health>().TakeDamage(damage);
                    UnityEngine.Debug.Log(damage.ToString() + " Damage Dealt");
                    AudioSource enemySource = enemyCollider.gameObject.GetComponentInChildren<AudioSource>();
                    enemySource.Play();

                    attackButtonHandler.SpawnDamageNumbers(target, minColour, maxColour, false, damage);
                    if (target.GetComponent<Health>().isDead)
                    {
                        playerHasDied = true;
                        scoreHandler.playerScores[playerIdentifier.currentPlayerIndex].playersKilled++;
                        if (bulletNum == 0)
                        {
                            Debug.Log(playerHasDied);
                            cardHandler.DrawCard(playerIdentifier.currentPlayer, 1, true, true);
                        }
                        else
                        {
                            cardHandler.DrawCard(playerIdentifier.currentPlayer, 1, false, true);
                        }

                    }
                    numberRoller.TurnOffNumberRoller();
                    if (bulletNum == 0)
                    {
                        if (!playerHasDied)
                        {
                            CardHandler cardHandler = FindObjectOfType<CardHandler>();
                            cardHandler.StartCoroutine(cardHandler.DelayNextTurn());
                        }
                    }

                    playerIdentifier.currentAttack.projectileIsFiring = false;

                    if (attackWeapon.GetComponent<Weapon>().weaponType == Weapon.weaponClass.Heavy)
                    {
                        transform.parent.GetComponentInChildren<HeavyAttack>().Reverse();
                    }
                    else if (attackWeapon.GetComponent<Weapon>().weaponType == Weapon.weaponClass.Light)
                    {
                        GetComponentInChildren<LightAttack>().PlayVFX();
                    }
                    else
                    {
                        Destroy(transform.parent.gameObject);
                    }

                    hasAttacked = true;
                }
            }
        }
        public void SetTarget(GameObject _target, GameObject _instigator, int _damage, int min, int max, AttackButtonHandler origin, int num, GameObject _attackWeapon)
        {
            attackWeapon = _attackWeapon;
            target = _target;
            instigator = _instigator;
            damage = _damage;
            minColour = min;
            maxColour = max;
            attackButtonHandler = origin;
            bulletNum = num;
            _target.GetComponentInChildren<AudioSource>().clip = ImpactClip;
            transform.LookAt(target.transform);

            if (attackWeapon.GetComponent<Weapon>().weaponType == Weapon.weaponClass.Medium)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance < maxDistanceThreshold)
                {
                    float slowFactor = CalculateSlowFactor(distance);

                    // Apply the slow factor to the speed
                    projectileSpeed *= slowFactor;
                }
            }
        }

        private float CalculateSlowFactor(float distance)
        {
            // Define the parameters of the slow function
            float minDistance = 1f;   // Minimum distance at which the projectile is slowed down
            float maxDistance = 20f;  // Maximum distance at which the projectile is slowed down
            float maxSlowFactor = 0.4f;  // Maximum slow factor applied to the speed

            // Check if the distance is within the desired range
            if (distance > maxDistance)
            {
                return 1f; // No slowdown
            }

            // Calculate the slow factor based on the distance
            float normalizedDistance = Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));
            float slowFactor = 1f - normalizedDistance * maxSlowFactor;

            return slowFactor;
        }
    }

}