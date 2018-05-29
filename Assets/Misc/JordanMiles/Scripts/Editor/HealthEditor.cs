using UnityEditor;
using UnityEngine;

namespace JMiles42.Editor.Editors {
    [CustomEditor(typeof(Health)), CanEditMultipleObjects]
    public class HealthEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.DrawDefaultInspector();
            var health = target as Health;

            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            var rH = EditorGUILayout.BeginVertical();
            EditorGUI.ProgressBar(rH, health.Amount / health.maxAmount, "Health");
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();

            GUI.backgroundColor = backgroundColor;
            rH = EditorGUILayout.BeginVertical();
            if (GUI.Button(rH, "Heal To Full health")) {
                health.Change(health.maxAmount);
            }
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();
        }
    }
}