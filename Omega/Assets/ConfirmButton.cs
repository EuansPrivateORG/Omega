using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Omega.UI
{
    public class ConfirmButton : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI confirmText;
        [SerializeField] public Color onColour;
        [SerializeField] public Color offColour;

        private Button confirmButton;

        private void Awake()
        {
            confirmButton = gameObject.GetComponent<Button>();
        }
        private void Update()
        {

            if (confirmButton.interactable == false)
            {
                confirmText.color = offColour;
            }
            else 
            {
                confirmText.color = onColour;
            }
        }
    }
}
