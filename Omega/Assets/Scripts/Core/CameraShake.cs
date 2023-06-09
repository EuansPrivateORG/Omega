using Cinemachine;
using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Omega.UI
{
    public class CameraShake : MonoBehaviour
    {
        public float shakeTimer = 10;
        public float intensity = 1f;
        public float rumbleDuration = 0.5f;

        CinemachineVirtualCamera cinemachineVirtualCamera;

        private void Awake()
        {
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                if (shakeTimer <= 0f)
                {
                    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
                }
            }
        }

        public void ShakeCamera(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            cinemachineBasicMultiChannelPerlin.m_FrequencyGain = time;
            shakeTimer = time;


        }

        public IEnumerator RumbleCoroutine(Gamepad gamepad, float intensity, float duration)
        {
            const float motorSpeedMax = 1f;
            const int rumbleFrequency = 10;

            // Set the motor speeds to the maximum
            gamepad.SetMotorSpeeds(motorSpeedMax, motorSpeedMax);

            // Calculate the delay between motor speed decreases
            float delay = duration / rumbleFrequency;

            // Gradually decrease the motor speeds over time
            for (int i = 0; i < rumbleFrequency; i++)
            {
                float t = (float)i / rumbleFrequency;
                float motorSpeed = Mathf.Lerp(intensity, 0f, t);
                gamepad.SetMotorSpeeds(motorSpeed, motorSpeed);
                yield return new WaitForSeconds(delay);
            }

            // Ensure the motor speeds are set to 0 at the end
            gamepad.SetMotorSpeeds(0f, 0f);
        }
    }
}
