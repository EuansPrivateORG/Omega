using UnityEngine;

namespace Omega.Combat
{
    public class WeaponRotation : MonoBehaviour
    {
        public bool rotateEnabled = false;
        public float rotationSpeed = 1f;
        public float maxRotationAngle = 60f;

        private Quaternion startRotation;
        private Quaternion targetRotation;

        private void Start()
        {
            // Store the initial rotation
            startRotation = transform.rotation;
        }

        private void Update()
        {
            if (rotateEnabled)
            {
                // Calculate the target rotation based on the current rotation
                float targetAngle = Mathf.PingPong(Time.time * rotationSpeed, maxRotationAngle * 2f) - maxRotationAngle;
                Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

                // Perform rotation towards the target rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, startRotation * targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        public void ToggleRotation()
        {
            rotateEnabled = !rotateEnabled;
        }
    }
}
