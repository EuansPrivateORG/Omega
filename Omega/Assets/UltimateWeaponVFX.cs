using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltimateWeaponVFX : MonoBehaviour
{
    public GameObject initialPrefab; // Prefab to be instantiated initially
    public GameObject delayedPrefab; // Prefab to be instantiated after the delay
    public int numberOfPrefabs; // Number of prefabs to be instantiated
    public float radius; // Initial radius for instantiation
    public float rotationSpeed; // Speed of rotation in degrees per second
    public float radiusDecreaseRate; // Rate at which radius decreases per second
    public float destroyThreshold; // Threshold radius at which prefabs are destroyed
    public float instantiateDelay; // Delay between destroying and instantiating new prefabs

    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private float currentRadius;
    private bool isRotating = true;

    private void Start()
    {
        currentRadius = radius;
        InstantiatePrefabs(initialPrefab);
    }

    private void Update()
    {
        if (isRotating)
        {
            RotatePrefabs();
            DecreaseRadius();
        }
    }

    private void InstantiatePrefabs(GameObject selectedPrefab)
    {
        for (int i = 0; i < numberOfPrefabs; i++)
        {
            float angle = i * (360f / numberOfPrefabs);
            Vector3 spawnPosition = transform.position + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * currentRadius;
            GameObject instantiatedPrefab = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
            instantiatedPrefabs.Add(instantiatedPrefab);
        }
    }

    private void RotatePrefabs()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationAmount);
    }

    private void DecreaseRadius()
    {
        currentRadius -= radiusDecreaseRate * Time.deltaTime;
        if (currentRadius <= destroyThreshold)
        {
            isRotating = false;
            DestroyPrefabs();
            StartCoroutine(InstantiateAfterDelay());
        }
    }

    private void DestroyPrefabs()
    {
        foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
        {
            Destroy(instantiatedPrefab);
        }
        instantiatedPrefabs.Clear();
    }

    private IEnumerator InstantiateAfterDelay()
    {
        yield return new WaitForSeconds(instantiateDelay);
        isRotating = true;
        currentRadius = radius;
        InstantiatePrefabs(delayedPrefab);
    }
}

