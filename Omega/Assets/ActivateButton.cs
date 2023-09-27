using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Omega.UI
{
    public class ActivateButton : MonoBehaviour
    {
        private Button activateButton;
        private ColorBlock buttonColors;
        public bool canLerp = true;

        private void Awake()
        {
            activateButton = GetComponent<Button>();
            buttonColors = activateButton.colors;
        }

        private void Update()
        {
            if (canLerp)
            {
                float lerpAlpha = Mathf.PingPong(Time.time, 1f);
                float alphaValue = 1f - lerpAlpha;
                buttonColors.selectedColor = new Color(buttonColors.selectedColor.r, buttonColors.selectedColor.g, buttonColors.selectedColor.b, alphaValue);
                activateButton.colors = buttonColors;

            }
        }

        public void stopLerp()
        {
            canLerp = false;
        }
    }
}
