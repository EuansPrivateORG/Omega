using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Omega.UI
{
    public class ActionButtonHandler : MonoBehaviour
    {
        [HideInInspector]
        public Button button;

        public void SetSelectedElement(Selectable selectable)
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
        }


    }
}
