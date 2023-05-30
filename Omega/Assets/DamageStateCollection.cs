using Omega.Core;
using Omega.Status;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageStateCollection : MonoBehaviour
{
    [SerializeField] public List<GameObject> damageStates;
    PlayerSpawnHandler playerSpawnHandler;

    private void Awake()
    {
        playerSpawnHandler = FindObjectOfType<PlayerSpawnHandler>();
    }

    public void CheckHealth()
    {
        Health playerHealth = gameObject.GetComponent<Health>();
        float healthPercent = ((float)playerHealth.currentHealth / (float)playerSpawnHandler.playerStartingHealth) * 100f;

        if (healthPercent > 75f)
        {
            DisableDamageState(0);
        }
        else if (healthPercent <= 75f && healthPercent > 50f)
        {
            EnableDamageState(0);
            DisableDamageState(1);
        }
        else if (healthPercent <= 50f && healthPercent > 35f)
        {
            EnableDamageState(1);
            DisableDamageState(2);
        }
        else if (healthPercent <= 35f && healthPercent > 15f)
        {
            EnableDamageState(2);
            DisableDamageState(3);
        }
        else if (healthPercent <= 15f && healthPercent > 1f)
        {
            EnableDamageState(3);
            DisableDamageState(4);
        }
        else if (healthPercent <= 1f)
        {
            EnableDamageState(4);
        }
    }


    public void EnableDamageState(int stateNum)
    {
        for (int i = 0; i < damageStates.Count; i++)
        {
            if(i <= stateNum)
            {
                DamageState damageState = damageStates[i].GetComponent<DamageState>();
                foreach (ParticleSystem system in damageState.particles)
                {
                    system.Play();
                }
            }
        }
    }

    public void DisableDamageState(int stateNum)
    {
        for (int i = 0; i < damageStates.Count; i++)
        {
            if (i >= stateNum)
            {
                DamageState damageState = damageStates[i].GetComponent<DamageState>();
                foreach (ParticleSystem system in damageState.particles)
                {
                    system.Stop();
                }
            }
        }
    }
}
