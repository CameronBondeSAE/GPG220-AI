using UnityEngine;
using UnityEditor;

namespace JMiles42.Editor.Editors {
    //[CanEditMultipleObjects]
    //[CustomEditor(typeof(Transform))]
    public class TransformEditor : UnityEditor.Editor {
        private static bool scaleToggle;
        private static float scaleAmount = 1;
        private bool foldout;

        public override void OnInspectorGUI() {
            var transform = target as Transform;
            serializedObject.Update();
            foldout = EditorGUILayout.Foldout(foldout, "Copy & Paste");
            if (foldout) {
                EditorGUILayout.BeginHorizontal("Box");
                transform = EditorHelpers.CopyPastObjectButtons(transform) as Transform;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            var ResetContent = new GUIContent("Reset Transform", "Reset Transforms in global space");
            var ResetLocalContent = new GUIContent("Reset Local Transform", "Reset Transforms in local space");
            if (GUILayout.Button(ResetContent)) {
                Undo.RecordObject(transform, "ResetPosRotScale");
                transform.ResetPosRotScale();
            }
            if (GUILayout.Button(ResetLocalContent)) {
                Undo.RecordObject(transform, "ResetLocalPosRotScale");
                transform.ResetLocalPosRotScale();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            scaleToggle = EditorGUILayout.Toggle("Scale Presets", scaleToggle);
            if (scaleToggle) ScaleBtnsEnabled();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();


            EditorHelpers.Label(transform.parent == null ? "Transform" : "Local Transform");

            var localEulerAngles = EditorHelpers.DrawVector3("Rotation", transform.localEulerAngles, Vector3.zero, transform);
            var localPosition = EditorHelpers.DrawVector3("Position", transform.localPosition, Vector3.zero, transform);
            var localScale = EditorHelpers.DrawVector3("Scale   ", transform.localScale, Vector3.one, transform);
            if (transform.localEulerAngles != localEulerAngles){
                Undo.RecordObject(transform, "localEulerAngles Changed");
                transform.localEulerAngles = localEulerAngles;
            }
            if (transform.localPosition != localPosition){
                Undo.RecordObject(transform, "localPosition Changed");
                transform.localPosition = localPosition;
            }
            if (transform.localScale != localScale){
                Undo.RecordObject(transform, "localScale Changed");
                transform.localScale = localScale;
            }


            serializedObject.ApplyModifiedProperties();
        }

        private void ScaleBtnsEnabled() {
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            ScaleArea();
            EditorGUILayout.BeginHorizontal();
            ScaleBtn(0.5f);
            ScaleBtn(1);
            ScaleBtn(2);
            ScaleBtn(5);
            ScaleBtn(10);
            ScaleBtn(20);
            ScaleBtn(50);
            ScaleBtn(100);
            EditorGUILayout.EndHorizontal();
        }

        private void ScaleBtn(float multi = 1) {
            var resetContent = new GUIContent(string.Format("{0}x", multi),
                string.Format("Resets the vector to ({0},{0},{0})", multi));

            if (GUILayout.Button(resetContent)) {
                var transform = target as Transform;
                Undo.RecordObject(transform, "Scale reset");
                transform.localScale = Vector3.one * multi;
                scaleAmount = transform.localScale.x;
            }
        }

        private void ScaleArea() {
            var transform = target as Transform;
            EditorGUILayout.BeginHorizontal();
            var content = new GUIContent("Scale amount", "Set amount to uniformly scale the object");
            scaleAmount = EditorGUILayout.FloatField(content, scaleAmount);
            var scaleContent = new GUIContent("Set Scale",
                string.Format("Sets the scale ({0},{0},{0})", scaleAmount));
            if (GUILayout.Button(scaleContent)) {
                Undo.RecordObject(transform, "Scale set");
                transform.localScale = Vector3.one * scaleAmount;
            }
            var scaleTimesContent = new GUIContent("Times Scale",
                string.Format("Sets the scale ({0},{1},{2})", transform.position.x * scaleAmount,
                    transform.position.y * scaleAmount, transform.position.z * scaleAmount));
            if (GUILayout.Button(scaleTimesContent)) {
                Undo.RecordObject(transform, "Scale set");
                transform.localScale *= scaleAmount;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}