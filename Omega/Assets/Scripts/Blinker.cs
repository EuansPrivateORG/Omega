using UnityEngine;

public class Blinker : MonoBehaviour
{
    public float interval = 1.0f; // Time interval in seconds
    private float timer = 0.0f;
    private bool isObjectActive = true;
    private GameObject targetObject;

    private void Start()
    {
        targetObject = gameObject; // You can assign the target GameObject here or in the Unity editor
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            ToggleObject();
            timer = 0.0f;
        }
    }

    private void ToggleObject()
    {
        isObjectActive = !isObjectActive;
        targetObject.SetActive(isObjectActive);
    }
}
