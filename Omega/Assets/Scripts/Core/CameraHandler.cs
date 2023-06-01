using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Omega.Core
{
    public class CameraHandler : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField] List<CinemachineVirtualCamera> playerCameras;
        [SerializeField] CinemachineVirtualCamera endRoundCam;

        [HideInInspector]
        public CinemachineVirtualCamera firstPlayerCamera;
        private CinemachineVirtualCamera currentPlayerCamera;

        PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        }

        public void SetupCameras()
        {
            playerCameras = GetAllPlayerCameras();
            firstPlayerCamera = playerIdentifier.currentPlayer.GetComponentInChildren<CinemachineVirtualCamera>();
            currentPlayerCamera = firstPlayerCamera;

            for (int i = 0; i < playerCameras.Count; i++)
            {
                if (playerCameras[i] == currentPlayerCamera) playerCameras[i].Priority = 20;
                else playerCameras[i].Priority = 10;
            }
        }

        public List<CinemachineVirtualCamera> GetAllPlayerCameras()
        {
            List<CinemachineVirtualCamera> playerCameras = new List<CinemachineVirtualCamera>(FindObjectsOfType<CinemachineVirtualCamera>());
            return playerCameras;
        }

        public void SwitchCamera(CinemachineVirtualCamera nextPlayerCamera)
        {
            currentPlayerCamera = nextPlayerCamera;
            nextPlayerCamera.Priority = 20;

            for (int i = 0; i < playerCameras.Count; i++)
            {
                if (playerCameras[i] != currentPlayerCamera) playerCameras[i].Priority = 10;
            }
        }

        public void EndRoundCam()
        {
            SwitchCamera(endRoundCam);
        }
    }
}
