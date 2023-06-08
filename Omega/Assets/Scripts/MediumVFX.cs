using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumVFX : MonoBehaviour
{
    public ParticleSystem explosion;
    public GameObject target;
    public float moveSpeed;
    public bool hasPlayedVFX = false;
    public float explosionThreshold;

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(target.transform);

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        if (distanceToTarget <= explosionThreshold)
        {
            if (!hasPlayedVFX)
            {
                explosion.Play();
                hasPlayedVFX = true;
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
