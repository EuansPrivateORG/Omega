using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Omega.UI
{
    public class PlayerOutlineHandler : MonoBehaviour
    {
        EventSystem eventSystem;

        private void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }

        private void Update()
        {
            if(eventSystem.currentSelectedGameObject == gameObject)
            {
                GetComponent<Outline>().OutlineColor = Color.red;
            }
            else
            {
                GetComponent<Outline>().OutlineColor = Color.white;
            }
        }
    }
}
