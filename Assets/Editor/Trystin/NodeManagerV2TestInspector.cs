using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Trystin
{
    [CustomEditor(typeof(NodeManagerV2))]
    public class NodeManagerV2TestInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            NodeManagerV2 TargetTestNMV2 = (NodeManagerV2)target;

            if (GUILayout.Button("Get Floor Dimentions"))
            {
                TargetTestNMV2.ResetGrid();
                TargetTestNMV2.GetFloorGridDimentions();
            }
            if (GUILayout.Button("SetUpGrid"))
            {
                TargetTestNMV2.ResetGrid();
                TargetTestNMV2.CallCreateGrid();
            }
            if (GUILayout.Button("Remove Non-Floor Tiles"))
            {
                TargetTestNMV2.RemoveNonFloorTiles();
            }
        }
    }
}
