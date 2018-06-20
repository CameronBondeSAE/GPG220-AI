using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Trystin
{
    [CustomEditor(typeof(Artillery))]
    public class GunCrewTestInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Artillery TargetTestGunCrewAI = (Artillery)target;

            if (GUILayout.Button("Request GunSpawn"))
            {
                if (NodeManager.Instance.SetupCompletate)
                    TargetTestGunCrewAI.RequestSpawnIn();
                else
                    Debug.Log("Wait for NodeManager Setup is compleate");
            }
        }
    }
}

