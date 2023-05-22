using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CrystalGen : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private int prefabCount = 10;
    private Vector3 spawnPosition;
    private float radius = 10f;

    [MenuItem("Window/Prefab Generator")]
    public static void ShowWindow()
    {
        GetWindow<CrystalGen>("Prefab Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Generator", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Prefabs");

        for (int i = 0; i < prefabs.Count; i++)
        {
            prefabs[i] = EditorGUILayout.ObjectField("Prefab " + (i + 1), prefabs[i], typeof(GameObject), false) as GameObject;
        }

        if (GUILayout.Button("Add Prefab"))
        {
            prefabs.Add(null);
        }

        prefabCount = EditorGUILayout.IntField("Prefab Count", prefabCount);
        spawnPosition = EditorGUILayout.Vector3Field("Spawn Position", spawnPosition);
        radius = EditorGUILayout.FloatField("Radius", radius);

        if (GUILayout.Button("Generate Prefabs"))
        {
            GeneratePrefabs();
        }
    }

    private void GeneratePrefabs()
    {
        if (prefabs.Count == 0)
        {
            Debug.LogError("No prefabs assigned!");
            return;
        }

        float angleIncrement = 360f / prefabCount;

        for (int i = 0; i < prefabCount; i++)
        {
            float angle = i * angleIncrement;
            Vector3 position = GetPositionOnCircle(angle, radius);

            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject spawnedPrefab = PrefabUtility.InstantiatePrefab(randomPrefab) as GameObject;
            spawnedPrefab.transform.position = spawnPosition + position;
        }
    }

    private Vector3 GetPositionOnCircle(float angle, float radius)
    {
        float radian = angle * Mathf.Deg2Rad;
        float x = Mathf.Sin(radian) * radius;
        float z = Mathf.Cos(radian) * radius;
        return new Vector3(x, 0f, z);
    }
}
