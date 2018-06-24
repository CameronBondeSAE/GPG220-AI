using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Trystin
{
    [CustomEditor(typeof(CommandManager))]
    public class CommandManagerTestInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CommandManager TargetTestCommanManger = (CommandManager)target;

            if (GUILayout.Button("Test MovmentCommand"))
            {
                if (NodeManager.Instance.SetupCompletate)
                    TargetTestCommanManger.TestMovementCommand();
                else
                    Debug.Log("Wait for NodeManager Setup is compleate");
            }
        }
    }
}
