using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobSpin : MonoBehaviour
{
    public float spinSpeed = 10f; // Speed of rotation in degrees per second
    public float bobSpeed = 1f; // Speed of bobbing in units per second
    public float bobAmount = 1f; // Amplitude of bobbing motion

    private float originalY; // Initial Y position of the game object

    // Start is called before the first frame update
    void Start()
    {
        originalY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Spin the game object around its up axis
        transform.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);

        // Calculate the new Y position based on time
        float newY = originalY + Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        // Update the game object's position
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}