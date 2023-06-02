using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumAttack : MonoBehaviour
{
    [SerializeField] GameObject mediumAttack;
    public int amountOfShots;
    [HideInInspector] public GameObject target;
    public float delayBetweenShots;
    public List<Transform> bulletSpawns;
    public float shotWaitTime;
    PlayerIdentifier playerIdentifier;

    void Start()
    {
        playerIdentifier = FindObjectOfType<PlayerIdentifier>();
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
            bullet.GetComponent<MediumVFX>().target = target;
            bulletSpawnCounter++;
            yield return new WaitForSeconds(delayBetweenShots);
        }

        playerIdentifier.currentAttack.continueWithAttack = true;
    }
}
