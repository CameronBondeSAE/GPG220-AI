using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Trystin
{
    [CustomEditor(typeof(PathFinderManager))]
    public class PFMTestInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PathFinderManager TargetTestPFM = (PathFinderManager)target;

            if (GUILayout.Button("Test Direction Search"))
            {
                if (NodeManager.Instance.SetupCompletate)
                {
                    TargetTestPFM.TestDirectionList = TargetTestPFM.ReturnDirectionalSearchNode(NodeManager.Instance.FindNodeFromWorldPosition(TargetTestPFM.TestTransform.position), TargetTestPFM.RadiusOfSearch, TargetTestPFM.SearchDirection);
                }
                else
                    Debug.Log("Wait for NodeManager Setup is compleate");
            }
        }
    }
}