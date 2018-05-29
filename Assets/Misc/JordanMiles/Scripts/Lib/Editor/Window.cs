using UnityEditor;
using UnityEngine;

namespace JMiles42.Editor {
    /// <summary>
    /// [MenuItem("Tools/JMiles42/Example Window")]
    /// private static void Init(){
    /// GetWindow();
    /// }
    /// </summary>
    /// <typeparam name="T">Class name of type that inherets directly from this class, for a static ref to its self</typeparam>
    public class Window<T> : EditorWindow where T : EditorWindow {
        protected static T window;

        protected static void GetWindow() {
            // Get existing open window or if none, make a new one:
            window = GetWindow<T>();
        }

        protected virtual void OnGUI() {}

        protected virtual void Update() {
            //Repaint();
        }

        public static void DrawTitle(string Title) {
            var gui = new GUIStyle {fontSize = 21};
            EditorGUILayout.LabelField(new GUIContent(Title), gui);
        }

        public struct EditorColour {
            public Color Color;
            public Color lightColor;

            public EditorColour(Color color) {
                Color = color;
                lightColor = color * 0.7f;
                lightColor.a = 1f;
            }

            public EditorColour(Color color, float lcolor) {
                Color = color;
                lightColor = color * lcolor;
                lightColor.a = 1f;
            }

            public EditorColour(Color color, Color lcolor) {
                Color = color;
                lightColor = lcolor;
            }

            public static implicit operator Color(EditorColour editorColour) {
                return EditorGUIUtility.isProSkin ? editorColour.Color : editorColour.lightColor;
            }
        }
    }
}