using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumVFX : MonoBehaviour
{
    public GameObject target;
    public float moveSpeed;

    void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(target.transform);
    }
}
