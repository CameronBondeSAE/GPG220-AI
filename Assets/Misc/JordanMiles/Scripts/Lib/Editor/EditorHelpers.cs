using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JMiles42.Editor {
    public class EditorHelpers : UnityEditor.Editor {
        public static Vector3 DrawVector3(string label, Vector3 vec, Vector3 defaultValue, Object objectIAmOn,
            bool allowTransformDrop = false) {
            return DrawVector3(new GUIContent(label, "The vectors X,Y,Z values."), vec, defaultValue, objectIAmOn, allowTransformDrop);
        }

        public static Vector3 DrawVector3(GUIContent label, Vector3 vec, Vector3 defaultValue, Object objectIAmOn,
            bool allowTransformDrop = false) {
            EditorGUILayout.BeginHorizontal();


            vec = EditorGUILayout.Vector3Field(label, vec);


            if (allowTransformDrop) {
                var transformContent = new GUIContent("", "Assign the vectors value from a transform Position");
                Transform transform = null;

                transform =
                    (Transform)EditorGUILayout.ObjectField(transformContent, transform, typeof(Transform), true,
                        GUILayout.Width(50));

                if (transform != null) {
                    EditorGUILayout.EndHorizontal();
                    return transform.position;
                }
            }

            var resetContent = new GUIContent("R", "Resets the vector to  " + defaultValue);
            if (GUILayout.Button(resetContent, GUILayout.Width(25))) {
                Undo.RecordObject(objectIAmOn, "Vector 3 Reset");
                vec = defaultValue;
            }
            var copyContent = new GUIContent("C", "Copies the vectors data.");
            if (GUILayout.Button(copyContent, GUILayout.Width(25))) CopyPaste.EditorCopy(vec);
            var pasteContent = new GUIContent("P", "Pastes the vectors data.");
            if (GUILayout.Button(pasteContent, GUILayout.Width(25))) {
                Undo.RecordObject(objectIAmOn,"Vector 3 Paste");
                vec = CopyPaste.Paste<Vector3>();
            }

            EditorGUILayout.EndHorizontal();
            return vec;
        }

        public static void Label(string label) {
            EditorGUILayout.LabelField(label, GUILayout.Width(GetStringLengthinPix(label)));
        }

        public static UnityEngine.Object CopyPastObjectButtons(UnityEngine.Object obj) {
            var CopyContent = new GUIContent("Copy Data", "Copies the data.");
            var PasteContent = new GUIContent("Paste Data", "Pastes the data.");
            if (GUILayout.Button(CopyContent)) {
                CopyPaste.EditorCopy(obj);
            }
            if (GUILayout.Button(PasteContent)) {
                Undo.RecordObject(obj,"Before Paste Settings");
                CopyPaste.EditorPaste(ref obj);
            }
            return obj;
        }

        public static float GetStringLengthinPix(string str) {
            return str.EditorStringWidth();
        }

        public static void DrawProperty(Rect position, EditorEntry serializedProperty) {
            EditorGUI.SelectableLabel(new Rect(position.x, position.y, serializedProperty, position.height),
                (serializedProperty));
            EditorGUI.PropertyField(
                new Rect(position.x + serializedProperty, position.y, position.width - serializedProperty,
                    position.height), serializedProperty, GUIContent.none);
        }

        public static void CreateAndCheckFolder(string path, string dir) {
            if (!AssetDatabase.IsValidFolder(path + "/" + dir)) AssetDatabase.CreateFolder(path, dir);
        }
    }

    public class EditorString {
        public readonly string String;
        public readonly float Length;

        public EditorString(string str) {
            String = str;
            Length = EditorHelpers.GetStringLengthinPix(String);
        }

        public static implicit operator float(EditorString editorString) {
            return editorString.Length;
        }

        public static implicit operator string(EditorString editorString) {
            return editorString.String;
        }
    }

    public class EditorEntry {
        public SerializedProperty Property;
        public readonly string String;
        public readonly float Length;

        public EditorEntry(string str, SerializedProperty prop) {
            Property = prop;
            String = str;
            Length = EditorHelpers.GetStringLengthinPix(String);
        }

        public EditorEntry(SerializedProperty prop) {
            Property = prop;
            String = prop.displayName;
            Length = EditorHelpers.GetStringLengthinPix(String);
        }

        public static implicit operator SerializedProperty(EditorEntry editorString) {
            return editorString.Property;
        }

        public static implicit operator float(EditorEntry editorString) {
            return editorString.Length;
        }

        public static implicit operator string(EditorEntry editorString) {
            return editorString.String;
        }

        public void Draw(Rect position, SplitType splitType = SplitType.TextSize) { Draw(position, splitType, GUIStyle.none,""); }
        public void Draw(Rect position, SplitType splitType, string toolTip) { Draw(position, splitType, GUIStyle.none, toolTip); }
        public void Draw(Rect position, SplitType splitType, GUIStyle labelStyle,string toolTip) {
            int indentLevel = EditorGUI.indentLevel;
            var labelRect = new Rect(position.x, position.y, Length, position.height);
            var fieldRect = new Rect(position.x + Length, position.y, position.width - Length, position.height);
            EditorGUI.indentLevel = 0;
            switch (splitType) {
                case SplitType.Half:
                    labelRect = new Rect(position.x, position.y, position.width/2, position.height);
                    fieldRect = new Rect(position.x + (position.width / 2), position.y, position.width / 2, position.height);
                    break;
                case SplitType.Third:
                    labelRect = new Rect(position.x, position.y, position.width / 3, position.height);
                    fieldRect = new Rect(position.x + (position.width / 3), position.y, (position.width / 3)*2, position.height);
                    break;
            }

            EditorGUI.LabelField(MakeRectHaveHeightBorder(labelRect), new GUIContent(String,toolTip), labelStyle);
            EditorGUI.PropertyField(MakeRectHaveHeightBorder(fieldRect), Property, new GUIContent("",toolTip));
            EditorGUI.indentLevel = indentLevel;
        }

        public static Rect MakeRectHaveHeightBorder(Rect rect) {
            rect.height -= 1f;
            rect.y += 1f;
            return rect;
        }

        public enum SplitType {
            TextSize,
            Half,
            Third
        }
    }
    public static class EditorClassExtensions {
        public static float EditorStringWidth(this string str) { return (str.Length * 8f) + 4; }
    }
}