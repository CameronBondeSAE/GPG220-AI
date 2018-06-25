using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Trystin
{
    [CustomEditor(typeof(SpawnerTest))]
    public class SpawnTestInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SpawnerTest TargetSpawnerTest = (SpawnerTest)target;

            if (GUILayout.Button("Spawn Artillery Piece"))
            {
                TargetSpawnerTest.SpawnArtilleryPiecePrefab();
            }
        }
    }
}
