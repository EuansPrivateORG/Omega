using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Omega.UI
{
    public class PlayerSelectionHandler : MonoBehaviour
    {
        [SerializeField] public Outline outline;
        private void Update()
        {
            //if (outline.enabled == true)
            //{
            //    ChangeSelection();
            //}
        }
        public void ChangeSelection()
        {
            if (!EventSystem.current.currentSelectedGameObject == this)
            {
                Debug.Log(EventSystem.current.currentSelectedGameObject + "RED");
                outline.OutlineColor = Color.white;
            }
            else
            {
                outline.OutlineColor = Color.red;
                Debug.Log(EventSystem.current.currentSelectedGameObject + "WHITE");
            }
        }



    }
}
