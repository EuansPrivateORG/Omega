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
    }
}
