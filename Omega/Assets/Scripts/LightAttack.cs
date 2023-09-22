using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAttack : MonoBehaviour
{
    public GameObject explosion;

    public float waitTime;

    private bool canDestroy = false;
    private float timer;

    public void PlayVFX()
    {
        explosion.GetComponent<ParticleSystem>().Play();
        canDestroy = true;
    }

    private void Update()
    {
        if (canDestroy)
        {
            timer += Time.deltaTime;
            if(timer > waitTime)
            {
                Destroy(transform.parent.parent.gameObject);
            }
        }
    }
}
