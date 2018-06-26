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
                    Quadrent Gotquad = NodeManager.Instance.FindQuadrentFromWorldPosition(TargetTestPFM.gameObject.transform.position);
                    Debug.Log(Gotquad.QuadrentPostion);
                }
                else
                    Debug.Log("Wait for NodeManager Setup is compleate");
            }
        }
    }
}