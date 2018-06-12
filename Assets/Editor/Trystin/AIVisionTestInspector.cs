using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Trystin
{
    [CustomEditor(typeof(TestVisionAI))]
    public class AIVisionTestInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TestVisionAI TargetTestMovementAI = (TestVisionAI)target;

            if (GUILayout.Button("Test Vision Range"))
            {
                if (NodeManager.Instance.SetupCompletate)
                    TargetTestMovementAI.SightRangeScan();
                else
                    Debug.Log("Wait for NodeManager Setup is compleate");
            }
        }
    }
}
