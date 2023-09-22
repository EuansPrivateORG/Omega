using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CurveEditor
{
    [CreateAssetMenu(fileName = "Curve Data", menuName = "Curve Data")]
    public class CurveEditorData : ScriptableObject
    {
        [SerializeField] public Curve[] curves;
    }
}