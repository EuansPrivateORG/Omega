using Omega.Combat;
using Omega.Core;
using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MediumAttack : MonoBehaviour
{
    [SerializeField] GameObject mediumAttack;
    [HideInInspector] public GameObject attackWeapon;
    public int amountOfShots;
    [HideInInspector] public GameObject target;
    public float delayBetweenShots;
    public List<Transform> bulletSpawns;
    public float shotWaitTime;
    PlayerIdentifier playerIdentifier;
    CameraShake cameraShake;
    Gamepad gamepad = Gamepad.current;

    void Start()
    {
        playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        cameraShake = GetComponentInParent<PlayerCam>().playerCam.GetComponent<CameraShake>();
        StartCoroutine(Shoot());
    }

    public IEnumerator Shoot()
    {
        int bulletSpawnCounter = 0;
        yield return new WaitForSeconds(shotWaitTime);


        for (int i = 0; i < amountOfShots; i++)
        {
            if(bulletSpawnCounter > bulletSpawns.Count - 1)
            {
                bulletSpawnCounter = 0;
            }
            GameObject bullet = Instantiate(mediumAttack, bulletSpawns[bulletSpawnCounter]);
            attackWeapon.GetComponent<Weapon>().muzzleFlash[bulletSpawnCounter].GetComponent<ParticleSystem>().Play();
            cameraShake.StartCoroutine(cameraShake.RumbleCoroutine(gamepad, cameraShake.intensity, 0.1f));
            bullet.GetComponent<MediumVFX>().target = target;
            bulletSpawnCounter++;
            yield return new WaitForSeconds(delayBetweenShots);
        }

        playerIdentifier.currentAttack.continueWithAttack = true;
    }
}
