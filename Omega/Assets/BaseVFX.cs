using Omega.Visual;
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
    public float timeBetweenStunFlash;
    private bool stunVFXActive = false;
    private GameObject currentStunImissive;
    private Color originalImissiveColor;


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
        if (stunVFXActive)
        {
            foreach (Transform t in currentStunImissive.transform)
            {
                Material mat = t.GetComponent<Renderer>().material;
                float lerpFactor = Mathf.PingPong(Time.time * timeBetweenStunFlash, 1f);
                Color targetColor = Color.Lerp(Color.black / 10, originalImissiveColor * 4, lerpFactor);
                mat.SetColor("_EmissionColor", targetColor);
            }
        }
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

    public void StartStunVFX()
    {
        currentStunImissive = gameObject.GetComponentInChildren<BaseCollection>().emissivePiecesParent;
        originalImissiveColor = currentStunImissive.GetComponentInChildren<Renderer>().material.GetColor("_EmissionColor");
        stunVFXActive = true;
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

    public void StunVFXStop()
    {
        stunVFXActive = false;
        foreach (Transform t in currentStunImissive.transform)
        {
            Material mat = t.GetComponent<Renderer>().material;
            mat.SetColor("_EmissionColor", originalImissiveColor);
        }
    }
}
