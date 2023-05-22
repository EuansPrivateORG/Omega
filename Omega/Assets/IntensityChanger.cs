using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.Visual
{
    public class IntensityChanger : MonoBehaviour
    {
        [SerializeField] List<GameObject> glowLights = new List<GameObject>();
        public float minIntensity = 0.5f;
        public float maxIntensity = 3f;
        public float minRange = 40f;
        public float maxRange = 150f;
        public float lerpDuration = 1f; // Duration of the lerping process

        private void Update()
        {
            LerpIntensity();
        }

        private void LerpIntensity()
        {
            float t = Mathf.PingPong(Time.time, lerpDuration) / lerpDuration; // Normalized time between 0 and 1

            foreach (GameObject light in glowLights)
            {
                Light glowLight = light.GetComponent<Light>();
                glowLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
                glowLight.range = Mathf.Lerp(minRange, maxRange, t);
            }
        }
    }
}
