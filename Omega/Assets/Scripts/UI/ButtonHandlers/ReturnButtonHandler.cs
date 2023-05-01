using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Omega.UI
{
    public class ReturnButtonHandler : MonoBehaviour
    {
        public void ChangeSelectedInput(Selectable newSelectedElement)
        {
            EventSystem.current.SetSelectedGameObject(newSelectedElement.gameObject);
        }
    }
}
