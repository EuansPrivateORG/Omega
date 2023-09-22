using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSelectionIdentifier : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI playerNumber;
    [SerializeField] public Image FactionIconImage;

    [SerializeField] public Button leftButton;
    [SerializeField] public Button rightButton;
    [SerializeField] public Button confirmButton;
}
