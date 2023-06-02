using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UltimateAttack : MonoBehaviour
{
    public GameObject initialPrefab; // Prefab to be instantiated initially
    public GameObject delayedPrefab; // Prefab to be instantiated after the delay
    public int numberOfPrefabs; // Number of prefabs to be instantiated
    public float radius; // Initial radius for instantiation
    public float rotationSpeed; // Speed of rotation in degrees per second
    public float radiusDecreaseRate; // Rate at which radius decreases per second
    public float destroyThreshold; // Threshold radius at which prefabs are destroyed
    public float instantiateDelay; // Delay between destroying and instantiating new prefabs
    public float lerpSpeed; // Speed at which the prefabs move towards the center
    public float destroyDelay;



    private List<GameObject> instantiatedPrefabs = new List<GameObject>();
    private GameObject parentObject;
    private GameObject finalLaser;
    private float currentRadius;
    private bool isRotating = true;
    private bool isLerping = false;
    private bool isDelayedPrefabInstantiated = false;

    private PlayerIdentifier playerIdentifier;


    private void Start()
    {
        playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        parentObject = new GameObject("VFXParent"); // Create a parent object
        parentObject.transform.position = transform.position;
        currentRadius = radius;
        InstantiatePrefabs(initialPrefab);
        playerIdentifier.currentAttack.continueWithAttack = true;
    }



    private void Update()
    {
        if (isRotating)
        {
            RotateParent();
            DecreaseRadius();
        }
        else if (isLerping)
        {
            RotateParent();
            LerpingPrefabs();
        }
    }



    private void InstantiatePrefabs(GameObject selectedPrefab)
    {
        Vector3 centerPosition = transform.position;
        float angleIncrement = 360f / numberOfPrefabs;



        for (int i = 0; i < numberOfPrefabs; i++)
        {
            float angle = i * angleIncrement;
            Vector3 spawnPosition = centerPosition + Quaternion.Euler(0f, angle, 0f) * Vector3.forward * currentRadius;
            GameObject instantiatedPrefab = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
            instantiatedPrefab.transform.SetParent(parentObject.transform); // Set the parent of the instantiated prefab
            instantiatedPrefabs.Add(instantiatedPrefab);
        }
    }



    private void RotateParent()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        parentObject.transform.Rotate(Vector3.up, rotationAmount);
    }



    private void DecreaseRadius()
    {
        currentRadius -= radiusDecreaseRate * Time.deltaTime;
        if (currentRadius <= destroyThreshold)
        {
            isRotating = false;
            isLerping = true;
        }
    }



    private void LerpingPrefabs()
    {
        Vector3 centerPosition = transform.position;
        float lerpStep = lerpSpeed * Time.deltaTime;



        foreach (GameObject instantiatedPrefab in instantiatedPrefabs)
        {
            instantiatedPrefab.transform.position = Vector3.Lerp(instantiatedPrefab.transform.position, centerPosition, lerpStep);



            if (!isDelayedPrefabInstantiated && Vector3.Distance(instantiatedPrefab.transform.position, centerPosition) <= 0.1f)
            {
                isDelayedPrefabInstantiated = true;
            }
        }



        if (isDelayedPrefabInstantiated)
        {
            isLerping = false;
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
        DestroyPrefabs();
        yield return new WaitForSeconds(instantiateDelay);
        isRotating = false;
        isLerping = false;
        isDelayedPrefabInstantiated = false;
        currentRadius = radius;

        Vector3 centerPosition = transform.position;
        finalLaser = Instantiate(delayedPrefab, centerPosition, Quaternion.identity);
        FindObjectOfType<WhiteFade>().StartFade();
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(finalLaser);
        Destroy(parentObject);
        Destroy(gameObject);
    }
}