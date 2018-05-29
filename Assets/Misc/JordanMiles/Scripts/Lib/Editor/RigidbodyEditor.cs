using UnityEngine;
using UnityEditor;

namespace JMiles42.Editor.Editors {
    //[CustomEditor(typeof(Rigidbody)), CanEditMultipleObjects]
    public class RigidbodyEditor : UnityEditor.Editor {
        public bool foldout = false;
        public override void OnInspectorGUI() {
            var body = target as Rigidbody;


            foldout = EditorGUILayout.Foldout(foldout,"Copy & Paste");
            if (foldout) {
                EditorGUILayout.BeginHorizontal("Box");
                body = EditorHelpers.CopyPastObjectButtons(body) as Rigidbody;
                EditorGUILayout.EndHorizontal();
            }

            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorHelpers.Label("Changing the Velocity may cause issues.");
            EditorGUILayout.Space();
            body.velocity = EditorHelpers.DrawVector3("Velocity", body.velocity, Vector3.zero, this, false);
            body.angularVelocity = EditorHelpers.DrawVector3("Angular", body.angularVelocity, Vector3.zero, this, false);
        }
    }
}