using Omega.Combat;
using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class HeavyAttack : MonoBehaviour
{
    [SerializeField] GameObject heavyAttackVFX;
    GameObject projectile;
    VolumetricLineBehavior volumetricLine; 
    PlayerIdentifier playerIdentifier;

    private void Start()
    {
        playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        projectile = GetComponentInChildren<Projectile>().gameObject;
        volumetricLine = heavyAttackVFX.GetComponent<VolumetricLineBehavior>();
    }

    private void OnDestroy()
    {
        playerIdentifier.currentAttack.continueWithAttack = true;
    }
}
