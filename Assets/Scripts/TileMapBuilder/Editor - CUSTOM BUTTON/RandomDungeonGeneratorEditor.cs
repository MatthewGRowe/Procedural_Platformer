using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AbstractPlatformGenerator), true)]
public class RandomDungeonGeneratorEditor : Editor
{
    AbstractPlatformGenerator generator;

    private void Awake()
    {
        generator = (AbstractPlatformGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Create Floor Platforms"))
        {
            generator.GenerateLevel();
        }
    }
}
