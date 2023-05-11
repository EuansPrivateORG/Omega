using Cinemachine;
using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Omega.UI
{
    public class CameraShake : MonoBehaviour
    {
        public float shakeTimer = 10;
        public float intensity = 1f;

        CinemachineVirtualCamera cinemachineVirtualCamera;
        PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }
        private void UpdateCamera()
        {
            cinemachineVirtualCamera = playerIdentifier.currentPlayer.GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if(shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                if(shakeTimer <= 0f)
                {
                    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
                }
            }
        }
        private void ShakeCamera(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            shakeTimer = time;
        }
    }
}
