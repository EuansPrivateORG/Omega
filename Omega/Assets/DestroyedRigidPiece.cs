using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedRigidPiece : MonoBehaviour
{
    public float objectLifetime = 5f;
    private float timer = 0f;
    private bool isKinematic = false;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isKinematic)
        {
            timer += Time.deltaTime;

            if (timer > objectLifetime)
            {
                rb.isKinematic = true;
                isKinematic = false;
            }
        }
    }
}
