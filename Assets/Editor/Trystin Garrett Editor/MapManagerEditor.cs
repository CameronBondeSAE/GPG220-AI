using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeManager))]
public class MapManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NodeManager TargetMapManger = (NodeManager)target;

        if(GUILayout.Button("Generate Map"))
        {
            TargetMapManger.CreateGrid();
        }
        if (GUILayout.Button("Delete Map"))
        {
            TargetMapManger.DeleteMap();
        }
    }
}
