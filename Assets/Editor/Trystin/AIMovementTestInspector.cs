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

        if (GUILayout.Button("Request Random Path"))
        {
            if (NodeManager.Instance.SetupCompletate)
                TargetTestMovementAI.RequestRandomPath();
            else
                Debug.Log("Wait for NodeManager Setup is compleate");
        }
    }
}
