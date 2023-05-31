using Omega.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class HeavyAttack : MonoBehaviour
{
    [SerializeField] GameObject heavyAttackVFX;
    GameObject projectile;
    VolumetricLineBehavior volumetricLine; 

    private void Start()
    {
        projectile = GetComponentInChildren<Projectile>().gameObject;
        volumetricLine = heavyAttackVFX.GetComponent<VolumetricLineBehavior>();
        //StartCoroutine(LookAtDelay(1f));
    }

    // Update is called once per frame
    void Update()
    {
        //heavyAttackVFX.transform.LookAt(projectile.transform);
    }

    private IEnumerator LookAtDelay(float time)
    {
        yield return new WaitForSeconds(time);
        heavyAttackVFX.transform.LookAt(projectile.transform);
    }
}
