using Omega.Visual;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseVFX : MonoBehaviour
{
    public GameObject healingVFX;
    public GameObject dotVFX;
    public GameObject scaraficeVFX;
    public GameObject hotVFX;
    public GameObject energyVFX;
    public GameObject overchargeVFX;
    public GameObject huntersVFX;
    public GameObject disruptorVFX;
    public GameObject explosionVFX;
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
        scaraficeVFX.GetComponent<ParticleSystem>().Stop();
        explosionVFX.GetComponent<ParticleSystem>().Stop();
        huntersVFX.SetActive(false);
        disruptorVFX.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (stunVFXActive)
        {
            foreach (Transform t in currentStunImissive.transform)
            {
                Material mat = t.GetComponent<Renderer>().material;
                float lerpFactor = Mathf.PingPong(Time.time * timeBetweenStunFlash, .25f);
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

    public void StartReticle()
    {
        huntersVFX.SetActive(true);
    }

    public void StartDisruptorVFX()
    {
        disruptorVFX.SetActive(true);
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

    public void StartSacraficeVFX()
    {
        scaraficeVFX.GetComponent<ParticleSystem>().Play();
        StartCoroutine(SacraficeVFXStop());
    }

    private IEnumerator SacraficeVFXStop()
    {
        yield return new WaitForSeconds(overchargeVFXTime);
        scaraficeVFX.GetComponent<ParticleSystem>().Stop();
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

    public void StopReticle()
    {
        huntersVFX.SetActive(false);
    }

    public void StopDisruptor()
    {
        disruptorVFX.SetActive(false);
    }

    public void PlayExplosion()
    {
        explosionVFX.GetComponent<ParticleSystem>().Play();
    }

    public void StopVFX()
    {
        healingVFX.GetComponent<ParticleSystem>().Stop();
        dotVFX.GetComponent<ParticleSystem>().Stop();
        hotVFX.GetComponent<ParticleSystem>().Stop();
        energyVFX.GetComponent<ParticleSystem>().Stop();
        overchargeVFX.GetComponent<ParticleSystem>().Stop();
        scaraficeVFX.GetComponent<ParticleSystem>().Stop();
        explosionVFX.GetComponent<ParticleSystem>().Stop();
        huntersVFX.SetActive(false);
        disruptorVFX.SetActive(false);
    }
}
