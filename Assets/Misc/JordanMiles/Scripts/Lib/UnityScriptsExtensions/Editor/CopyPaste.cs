using UnityEditor;
using UnityEngine;

namespace JMiles42.Editor {
    public static class CopyPaste {
        public static void EditorCopy<T>(T value) {
            EditorGUIUtility.systemCopyBuffer = EditorJsonUtility.ToJson(value, true);
        }

        public static void Copy<T>(T value) {
            EditorGUIUtility.systemCopyBuffer = JsonUtility.ToJson(value, true);
        }

        public static T Paste<T>() {
            T value = JsonUtility.FromJson<T>(EditorPaste());
            return value;
        }

        public static void EditorPaste<T>(ref T obj) {
            EditorJsonUtility.FromJsonOverwrite(EditorPaste(), obj);
        }

        public static void EditorPaste<T>(T obj) {
            EditorJsonUtility.FromJsonOverwrite(EditorPaste(), obj);
        }

        public static string EditorPaste() {
            return EditorGUIUtility.systemCopyBuffer;
        }
    }
}