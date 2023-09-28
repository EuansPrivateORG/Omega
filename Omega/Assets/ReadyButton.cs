using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] GameObject check;

    bool isReady = false;
    RoundStart roundStart;

    private void Awake()
    {
        roundStart = FindObjectOfType<RoundStart>();

        check.SetActive(false);
    }

    public void Ready()
    {
        if (!isReady)
        {
            isReady = true;
            check.SetActive(true);
            roundStart.Ready();
        }
        else if (isReady)
        {
            isReady = false;
            check.SetActive(false);
            roundStart.UnReady();
        }
    }

    public void UnReady()
    {
        isReady = false;
        check.SetActive(false);
        roundStart.UnReady();
    }
}
