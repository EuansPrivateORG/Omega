using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Omega.UI
{
    public class PlayerSelectionHandler : MonoBehaviour
    {
        public void SetSelectedElement(GameObject playerToSelect)
        {
            EventSystem.current.SetSelectedGameObject(gameObject.gameObject);
        }
    }
}
