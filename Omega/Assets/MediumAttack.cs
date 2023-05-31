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

    void Start()
    {
        StartCoroutine(Shoot());
    }

    public IEnumerator Shoot()
    {
        for (int i = 0; i < amountOfShots; i++)
        {
            GameObject bullet = Instantiate(mediumAttack, bulletSpawns[i]);
            bullet.GetComponent<MediumVFX>().target = target;
            yield return new WaitForSeconds(delayBetweenShots);
        }
    }
}
