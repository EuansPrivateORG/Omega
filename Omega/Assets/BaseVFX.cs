using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseVFX : MonoBehaviour
{
    public GameObject healingVFX;
    public GameObject dotVFX;
    public GameObject hotVFX;
    public GameObject energyVFX;
    public GameObject overchargeVFX;
    public float healingVFXTime;
    public float overchargeVFXTime;

    // Start is called before the first frame update
    void Start()
    {
        healingVFX.GetComponent<ParticleSystem>().Stop();
        dotVFX.GetComponent<ParticleSystem>().Stop();
        hotVFX.GetComponent<ParticleSystem>().Stop();
        energyVFX.GetComponent<ParticleSystem>().Stop();
        overchargeVFX.GetComponent<ParticleSystem>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PerformHealing()
    {
        healingVFX.GetComponent<ParticleSystem>().Play();
        StartCoroutine(HealingVFXStop());
    }

    private IEnumerator HealingVFXStop()
    {
        yield return new WaitForSeconds(healingVFXTime);
        healingVFX.GetComponent<ParticleSystem>().Stop();
    }
    public void StartOverchargeVFX()
    {
        overchargeVFX.GetComponent<ParticleSystem>().Play();
        StartCoroutine(OverchargeVFXStop());
    }

    private IEnumerator OverchargeVFXStop()
    {
        yield return new WaitForSeconds(overchargeVFXTime);
        overchargeVFX.GetComponent<ParticleSystem>().Stop();
    }

    public void StartDamageVFX()
    {
        dotVFX.GetComponent<ParticleSystem>().Play();
    }

    public void StartEnergyVFX()
    {
        energyVFX.GetComponent<ParticleSystem>().Play();
    }


    public void HOTVFXStart()
    {
        hotVFX.GetComponent<ParticleSystem>().Play();
    }

    public void HOTVFXStop()
    {
        hotVFX.GetComponent<ParticleSystem>().Stop();
    }

    public void DamageVFXStop()
    {
        dotVFX.GetComponent<ParticleSystem>().Stop();
    }

    public void EnergyVFXStop()
    {
        energyVFX.GetComponent<ParticleSystem>().Stop();
    }
}
