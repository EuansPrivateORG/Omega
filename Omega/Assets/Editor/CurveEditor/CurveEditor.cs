using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CurveEditor
{
    [Serializable]
    public class CurveEditor : EditorWindow
    {
        public CurveEditorData curveData;
        Curve curve = new Curve();
        List<Curve> curves = new List<Curve>();

        bool finishedEditingCurve = false;
        bool startedCreatingCurve = false;

        private string[] m_Tabs = { "Create Curve", "Created Curves" };
        private int m_TabsSelected = 0;

        Vector2 scrollPos = Vector2.zero;

        [MenuItem("Window/Curve Editor")]
        public static void OpenWindow()
        {
            CurveEditor window = EditorWindow.GetWindow<CurveEditor>("Curve Editor");
            window.minSize = new Vector2(475f, 500f);
            window.maxSize = new Vector2(475f, 800f);
            window.Show();
        }

        private void CreateGUI()
        {
            curves.AddRange(curveData.curves);

            finishedEditingCurve = false;
            startedCreatingCurve = false;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            m_TabsSelected = GUILayout.Toolbar(m_TabsSelected, m_Tabs);
            GUILayout.EndHorizontal();

            if(m_TabsSelected >= 0)
            {
                switch (m_Tabs[m_TabsSelected])
                {
                    case "Create Curve":
                        CreateCurve();
                        break;
                    case "Created Curves":
                        ViewSavedCurves();
                        break;
                }
            }

            if (startedCreatingCurve && !finishedEditingCurve)
            {
                EditorGUILayout.LabelField("Curve Settings", EditorStyles.boldLabel);
                curve.curveType = (Curve.CurveType)EditorGUILayout.EnumPopup(curve.curveType, GUILayout.Width(455));
                curve.startX = EditorGUILayout.FloatField("Start X", curve.startX, GUILayout.Width(455));
                curve.endX = EditorGUILayout.FloatField("End X", curve.endX, GUILayout.Width(455));
                if (curve.curveType == Curve.CurveType.constant)
                {
                    curve.y = EditorGUILayout.FloatField("Y Value", curve.y, GUILayout.Width(455));
                }
                else
                {
                    curve.startY = EditorGUILayout.FloatField("Start Y", curve.startY, GUILayout.Width(455));
                    curve.endY = EditorGUILayout.FloatField("End Y", curve.endY, GUILayout.Width(455));
                }
                if (curve.editingGraph)
                {
                    EditorGUILayout.LabelField("Graph Settings", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Graph settings are only while in this inspector and will not transfer when it is saved out", EditorStyles.miniLabel);
                    curve.curveColor = EditorGUILayout.ColorField("Curve Color", curve.curveColor, GUILayout.Width(455));
                    curve.minX = EditorGUILayout.FloatField("Min X", curve.minX, GUILayout.Width(455));
                    curve.minY = EditorGUILayout.FloatField("Min Y", curve.minY, GUILayout.Width(455));
                    curve.maxX = EditorGUILayout.FloatField("Max X", curve.maxX, GUILayout.Width(455));
                    curve.maxY = EditorGUILayout.FloatField("Max Y", curve.maxY, GUILayout.Width(455));
                }
                GUILayout.BeginHorizontal();
                if (!curve.editingGraph)
                {
                    if (GUILayout.Button("Edit Graph", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        curve.editingGraph = true;
                    }
                }
                else
                {
                    if (GUILayout.Button("Close Graph Settings", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        curve.editingGraph = false;
                    }
                }
                if (GUILayout.Button("Generate Curve", GUILayout.Width(150), GUILayout.Height(20)))
                {
                    switch (curve.curveType)
                    {
                        case Curve.CurveType.linear:
                            curve.curve1 = AnimationCurve.Linear(curve.startX, curve.startY, curve.endX, curve.endY);
                            break;
                        case Curve.CurveType.easeInOut:
                            curve.curve1 = AnimationCurve.EaseInOut(curve.startX, curve.startY, curve.endX, curve.endY);
                            break;
                        case Curve.CurveType.constant:
                            curve.curve1 = AnimationCurve.Constant(curve.startX, curve.startY, curve.y);
                            break;
                    }

                    curves.Insert(0, curve);

                    finishedEditingCurve = true;
                    curve.editingGraph = false;
                    startedCreatingCurve = false;
                    m_TabsSelected = 1;
                }
                if (GUILayout.Button("Reset Curve", GUILayout.Width(150), GUILayout.Height(20)))
                {
                    curve = new Curve();
                }
                GUILayout.EndHorizontal();
            }




            if (finishedEditingCurve)
            {
                EditorGUILayout.LabelField("Copy a saved curve out from the curve editor data to use it", EditorStyles.boldLabel, GUILayout.Width(400), GUILayout.Height(20));
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(position.height));

                List<Curve> curve2 = new List<Curve>();
                curve2.AddRange(curves);
                foreach (Curve _curve in curve2)
                {
                    GUILayout.BeginVertical(_curve.curveName, "window");
                    if (_curve.curveColor.a == 0)
                    {
                        _curve.curveColor = Color.red;
                    }
                    _curve.curve1 = EditorGUILayout.CurveField(_curve.curve1, _curve.curveColor, new Rect(_curve.minX, _curve.minY, _curve.maxX, _curve.maxY), GUILayout.Width(position.width - 20), GUILayout.Height(50));
                    _curve.curveName = EditorGUILayout.TextField("Curve Name", _curve.curveName, GUILayout.Width(455));

                    GUILayout.Space(2.5f);

                    if (!_curve.editingGraph)
                    {
                        if (GUILayout.Button("Edit Graph", GUILayout.Width(150), GUILayout.Height(20)))
                        {
                            _curve.editingGraph = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Close Graph Settings", GUILayout.Width(150), GUILayout.Height(20)))
                        {
                            _curve.editingGraph = false;
                        }
                    }

                    if (_curve.editingGraph)
                    {
                        EditorGUILayout.LabelField("Graph Settings", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("Graph settings are only while in this inspector and will not transfer when it is saved out", EditorStyles.miniLabel);
                        _curve.curveColor = EditorGUILayout.ColorField("Curve Color", _curve.curveColor);
                        _curve.minX = EditorGUILayout.FloatField("Min X", _curve.minX, GUILayout.Width(455));
                        _curve.minY = EditorGUILayout.FloatField("Min Y", _curve.minY, GUILayout.Width(455));
                        _curve.maxX = EditorGUILayout.FloatField("Max X", _curve.maxX, GUILayout.Width(455));
                        _curve.maxY = EditorGUILayout.FloatField("Max Y", _curve.maxY, GUILayout.Width(455));
                    }

                    GUILayout.Space(2.5f);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Smooth Curve", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        Curve curve = new Curve();
                        curve.curve1 = new AnimationCurve(_curve.curve1.keys);
                        curve.canBeDeleted = true;

                        float smoothAmount = 1f; // Scale the smooth amount to a 0-1 range

                        Keyframe[] keyframes = curve.curve1.keys;
                        Keyframe[] adjustedKeyframes = new Keyframe[keyframes.Length];

                        // Copy keyframes to adjustedKeyframes
                        for (int i = 0; i < keyframes.Length; i++)
                        {
                            adjustedKeyframes[i] = keyframes[i];
                        }

                        // Smooth keyframes using Catmull-Rom spline interpolation
                        for (int i = 1; i < adjustedKeyframes.Length - 1; i++)
                        {
                            Vector2 p0 = new Vector2(adjustedKeyframes[i - 1].time, adjustedKeyframes[i - 1].value);
                            Vector2 p1 = new Vector2(adjustedKeyframes[i].time, adjustedKeyframes[i].value);
                            Vector2 p2 = new Vector2(adjustedKeyframes[i + 1].time, adjustedKeyframes[i + 1].value);

                            // Calculate tangents using Catmull-Rom spline formula
                            float smoothingFactor = Mathf.Lerp(0.5f, 1f, smoothAmount); // Adjust the smoothing factor
                            Vector2 tangentIn = (p2 - p0) * (0.5f * (smoothAmount) * smoothingFactor);
                            Vector2 tangentOut = -tangentIn;

                            // Update the tangents and weights of the current keyframe
                            adjustedKeyframes[i].inTangent = tangentIn.y / tangentIn.x;
                            adjustedKeyframes[i].outTangent = tangentOut.y / tangentOut.x;
                            adjustedKeyframes[i].inWeight = smoothAmount;
                            adjustedKeyframes[i].outWeight = smoothAmount;
                        }

                        curve.curve1.keys = adjustedKeyframes;

                        int index = curves.IndexOf(_curve);
                        curves.Insert(index + 1, curve);
                    }

                    if (GUILayout.Button("Reverse X Axis", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        Curve curve = new Curve();
                        curve.curve1 = new AnimationCurve(_curve.curve1.keys);
                        curve.canBeDeleted = true;
                        Keyframe[] originalKeyframes = curve.curve1.keys;
                        int length = originalKeyframes.Length;
                        Keyframe[] reversedKeyframes = new Keyframe[length];

                        // Reverse the keyframe times and values while keeping the graph in the same position
                        float firstTime = originalKeyframes[0].time;
                        float lastTime = originalKeyframes[length - 1].time;
                        float timeRange = lastTime - firstTime;

                        for (int i = 0; i < length; i++)
                        {
                            Keyframe originalKeyframe = originalKeyframes[i];
                            Keyframe reversedKeyframe = new Keyframe(lastTime - (originalKeyframe.time - firstTime), originalKeyframe.value);
                            reversedKeyframe.inTangent = -originalKeyframe.inTangent;
                            reversedKeyframe.outTangent = -originalKeyframe.outTangent;
                            reversedKeyframes[length - 1 - i] = reversedKeyframe;
                        }

                        curve.curve1.keys = reversedKeyframes;

                        int index = curves.IndexOf(_curve);
                        curves.Insert(index + 1, curve);
                    }

                    if (GUILayout.Button("Reverse Y Axis", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        Curve curve = new Curve();
                        curve.curve1 = new AnimationCurve(_curve.curve1.keys);
                        curve.canBeDeleted = true;
                        Keyframe[] originalKeyframes = curve.curve1.keys;
                        int length = originalKeyframes.Length;
                        Keyframe[] reversedKeyframes = new Keyframe[length];

                        // Reverse the keyframe times and values while keeping the graph in the same position
                        float firstTime = originalKeyframes[0].time;
                        float lastTime = originalKeyframes[length - 1].time;
                        float timeRange = lastTime - firstTime;
                        float middleValue = (originalKeyframes[0].value + originalKeyframes[length - 1].value) * 0.5f;  // Calculate the middle value

                        for (int i = 0; i < length; i++)
                        {
                            Keyframe originalKeyframe = originalKeyframes[i];
                            Keyframe reversedKeyframe = new Keyframe(originalKeyframe.time, middleValue - (originalKeyframe.value - middleValue));  // Reverse the value while keeping it at the same height
                            reversedKeyframe.inTangent = -originalKeyframe.inTangent;
                            reversedKeyframe.outTangent = -originalKeyframe.outTangent;
                            reversedKeyframes[length - 1 - i] = reversedKeyframe;
                        }

                        curve.curve1.keys = reversedKeyframes;

                        int index = curves.IndexOf(_curve);
                        curves.Insert(index + 1, curve);
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.Space(2.5f);
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Save Curve", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        if (_curve.hasNotBeenSaved)
                        {
                            Curve[] _curves = new Curve[curveData.curves.Length + 1];
                            for (int i = 0; i < curveData.curves.Length; i++)
                            {
                                _curves[i] = curveData.curves[i];
                            }
                            int newPos = _curves.Length - 1;
                            _curves[newPos] = _curve;
                            curveData.curves = _curves;
                            _curve.hasNotBeenSaved = false;
                        }
                    }

                    if (GUILayout.Button("Delete Curve", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        curves.RemoveAll(curve => curve == _curve);
                        if (!_curve.hasNotBeenSaved || !_curve.canBeDeleted)
                        {
                            Curve[] _curves = new Curve[curveData.curves.Length - 1];
                            int index = 0;
                            for (int i = 0; i < curveData.curves.Length; i++)
                            {
                                if (curveData.curves[i] != _curve)
                                {
                                    _curves[index] = curveData.curves[i];
                                    index++;
                                }
                            }
                            curveData.curves = _curves;
                        }
                    }


                    if (GUILayout.Button("Duplicate Curve", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        Curve curve = new Curve();
                        curve.curve1 = new AnimationCurve(_curve.curve1.keys);
                        curve.canBeDeleted = true;

                        int index = curves.IndexOf(_curve);
                        curves.Insert(index + 1, curve);
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    GUILayout.EndVertical();

                    GUILayout.Space(10);
                }
                GUILayout.Space(75);
                GUILayout.EndScrollView();
            }
        }

        private void CreateCurve()
        {
            if (!startedCreatingCurve)
            {
                startedCreatingCurve = true;
                curve.editingGraph = false;

                curve = new Curve();
                curve.canBeDeleted = true;
                GUI.FocusControl(null);
            }
            if (finishedEditingCurve)
            {
                finishedEditingCurve = false;
                GUI.FocusControl(null);
            }
        }

        private void ViewSavedCurves()
        {
            if (!finishedEditingCurve)
            {
                curve.editingGraph = false;
                finishedEditingCurve = true;
                GUI.FocusControl(null);
            }
        }
    }
}