using Omega.Core;
using Omega.Status;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Omega.UI
{
    public class EnergyBar : MonoBehaviour
    {
        [SerializeField] GameObject segmentPrefab;
        public RectTransform spawnPosition;
        public Gradient segmentColourGradient;

        private PlayerIdentifier playerIdentifier;

        public void UpdateSegments()
        {
            DestroySegments();
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            Energy playerEnergy = playerIdentifier.currentPlayer.GetComponent<Energy>();
            int currentEnergyInt = playerEnergy.energy;

            for (int i = 0; i < currentEnergyInt; i++)
            {
              GameObject instantedSegment =  Instantiate(segmentPrefab, spawnPosition,false);
              instantedSegment.GetComponent<Image>().color = GetColorOnGradient(i, 1, 16, segmentColourGradient);
              instantedSegment.transform.SetAsFirstSibling();
            }
        }

        private void DestroySegments()
        {
            foreach (Transform child in spawnPosition.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public Color GetColorOnGradient(int value, int minValue, int maxValue, Gradient colorGradient)
        {
            float position = (float)(value - minValue) / (maxValue - minValue);
            return colorGradient.Evaluate(position);
        }
    }
}
