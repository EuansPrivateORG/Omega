using Omega.Combat;
using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using VolumetricLines;

public class HeavyAttack : MonoBehaviour
{
    [SerializeField] GameObject heavyAttackVFX;
    [SerializeField] float endPosZ = 10f;
    [SerializeField] float endPosY = 0.25f;
    [SerializeField] float endWidth = 1f;
    [SerializeField] float sizeUpTime = 1f;
    [SerializeField] float sizeDownTime = 0.5f;
    [HideInInspector] public float timer = 0f;
    private bool sizeDown = false;
    private bool sizeUp = true;
    VolumetricLineBehavior volumetricLine; 
    PlayerIdentifier playerIdentifier;

    private void Start()
    {
        playerIdentifier = FindObjectOfType<PlayerIdentifier>();
        volumetricLine = heavyAttackVFX.GetComponent<VolumetricLineBehavior>();
    }
    private void Update()
    {
        if (sizeUp)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / sizeUpTime);
            Vector3 lerpedPos = Vector3.Lerp(volumetricLine.EndPos, new Vector3(volumetricLine.EndPos.x, endPosY, endPosZ), t);
            volumetricLine.EndPos = lerpedPos;

            float lerpedWidth = Mathf.Lerp(0, endWidth, t);
            volumetricLine.LineWidth = lerpedWidth;
        }

        if (sizeDown)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / sizeDownTime);
            Vector3 lerpedPos = Vector3.Lerp(volumetricLine.StartPos, new Vector3(volumetricLine.EndPos.x, volumetricLine.EndPos.y, volumetricLine.EndPos.z - 0.1f), t);
            volumetricLine.StartPos = lerpedPos;

            float lerpedWidth = Mathf.Lerp(0, 1f, t);
            volumetricLine.LineWidth = lerpedWidth;

            if(volumetricLine.LineWidth == 1f && volumetricLine.StartPos == new Vector3(volumetricLine.EndPos.x, volumetricLine.EndPos.y, volumetricLine.EndPos.z - 0.1f))
            {
                Destroy(gameObject);
            }
        }
    }

    public void Reverse()
    {
        sizeDown = true;
        sizeUp = false;

        timer = 0;
    }

    private void OnDestroy()
    {
        playerIdentifier.currentAttack.continueWithAttack = true;
    }
}
