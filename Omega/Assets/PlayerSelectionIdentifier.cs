using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Omega.UI;

public class PlayerSelectionIdentifier : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI playerNumber;
    [SerializeField] public Image FactionIconImage;

    [SerializeField] public Button leftButton;
    [SerializeField] public Button rightButton;
    [SerializeField] public Button confirmButton;

    [HideInInspector] public int placeInFactionList;
    RoundStart roundStart;

    private void Awake()
    {
        roundStart = FindObjectOfType<RoundStart>();
    }

    public void UpFaction()
    {
        roundStart.UpFaction(this, placeInFactionList);
    }

    public void DownFaction()
    {
        roundStart.DownFaction(this, placeInFactionList);
    }
}
