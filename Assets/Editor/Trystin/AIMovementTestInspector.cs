using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestMovementAI))]
public class AIMovementTestInspector : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestMovementAI TargetTestMovementAI = (TestMovementAI)target;

        if (GUILayout.Button("Request Path"))
        {
            TargetTestMovementAI.RequestRandomPath();
        }
    }
}
