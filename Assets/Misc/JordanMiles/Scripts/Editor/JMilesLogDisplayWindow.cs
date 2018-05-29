using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JMiles42.Editor {
    public class JMilesLogDisplayWindow : Window<JMilesLogDisplayWindow> {
        [MenuItem("Tools/JMiles42/Logger")]
        private static void Init() {
            // Get existing open window or if none, make a new one:
            GetWindow();
            window.titleContent = new GUIContent("JMiles Log");

            window.Show();
            window.autoRepaintOnSceneChange = true;
            window.minSize = new Vector2(580, 350);
        }

        private Vector2 scrollPos = Vector2.zero;

        public bool ShowInfo = true;
        public bool ShowWarnings = true;
        public bool ShowError = true;
        public bool ShowException = true;
        public bool ShowAssert = true;

        protected override void Update() {
            Repaint();
        }

        protected override void OnGUI() {
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.magenta;
            EditorGUILayout.BeginHorizontal("box", GUILayout.Height(32));
            DrawTitle();
            GUI.backgroundColor = ShowInfo ? Color.green : Color.red;
            if (GUILayout.Button(new GUIContent("Info Logs", (ShowInfo ? "Enable Info Logs" : "Disable Info Logs")), GUILayout.Height(32), GUILayout.Width(70))) {
                ShowInfo = !ShowInfo;
            }

            GUI.backgroundColor = ShowWarnings ? Color.green : Color.red;
            if (GUILayout.Button(
                new GUIContent("Warnings", (ShowWarnings ? "Enable Warnings" : "Disable Warnings")),
                GUILayout.Height(32),
                GUILayout.Width(70))) {
                ShowWarnings = !ShowWarnings;
            }

            GUI.backgroundColor = ShowError ? Color.green : Color.red;
            if (GUILayout.Button(new GUIContent("Errors", (ShowError ? "Enable Errors" : "Disable Errors")), GUILayout.Height(32), GUILayout.Width(70))) {
                ShowError = !ShowError;
            }
            GUI.backgroundColor = ShowAssert ? Color.green : Color.red;
            if (GUILayout.Button(new GUIContent("Asserts", (ShowError ? "Enable Asserts" : "Disable Asserts")), GUILayout.Height(32), GUILayout.Width(70))) {
                ShowAssert = !ShowAssert;
            }
            GUI.backgroundColor = ShowException ? Color.green : Color.red;
            if (GUILayout.Button(new GUIContent("Exception", (ShowError ? "Enable Exceptions" : "Disable Exceptions")), GUILayout.Height(32), GUILayout.Width(70))) {
                ShowException = !ShowException;
            }

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = backgroundColor;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "box");
            if (JMilesDebug.debugData.debugLog.Count > 0)
                for (var index = 0; index < JMilesDebug.debugData.debugLog.Count; index++) {
                    DrawLog(JMilesDebug.debugData.debugLog[index], index);
                }
            EditorGUILayout.EndScrollView();
            DrawIOButtons();
        }

        private static void DrawTitle() { DrawTitle("JMiles Log Window"); }

        private void DrawLog(JMilesDebug.DebugLoggerData log, int index) {
            GetWindow();
            if (log == null) return;
            if (log.Info == null) return;
            if (log.Info.info == null) return;

            var backgroundColor = GUI.backgroundColor;
            switch (log.Info.rating) {
                case LogType.Error:
                    if (!ShowError) return;
                    GUI.backgroundColor = new EditorColour(Color.red*2, 0.8f);
                    break;
                case LogType.Assert:
                    if (!ShowAssert) return;
                    GUI.backgroundColor = new EditorColour(Color.blue, 0.8f);
                    break;
                case LogType.Warning:
                    if (!ShowWarnings) return;
                    GUI.backgroundColor = new EditorColour(Color.yellow);
                    break;
                case LogType.Log:
                    if (!ShowInfo) return;
                    GUI.backgroundColor = backgroundColor;
                    break;
                case LogType.Exception:
                    if (!ShowException) return;
                    GUI.backgroundColor = new EditorColour(Color.cyan, 0.8f);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            if (log.Expanded) {
                EditorGUILayout.BeginVertical("box", GUILayout.Height(27));
            }
            else {
                EditorGUILayout.BeginVertical("box", GUILayout.Height(27));
            }

            EditorGUILayout.BeginHorizontal();

            Rect r = EditorGUILayout.BeginHorizontal(GUILayout.Width(32), GUILayout.Height(27));
            log.Expanded = EditorGUI.Foldout(r, log.Expanded, "");
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            //log.Expanded = EditorGUILayout.Foldout(log.Expanded, "", true);
            var gui = new GUIStyle("label") {fontSize = 16};
            if (GUILayout.Button(new GUIContent(string.Format("[{0}] {1}", log.Count, log.Info.info)), gui, GUILayout.Width(window.position.width - 32 - 128 - 32))) {
                Select(log.Info.objectOfInfo);
            }

            if (GUILayout.Button(new GUIContent("Remove entry", "Deletes log from the database."), GUILayout.Height(20), GUILayout.Width(128))) {
                if (EditorUtility.DisplayDialog(
                    "WARNING: DELETE LOG ENTRY",
                    string.Format("Warning you are about to delete Log ({0}), Are you sure you want to do this?", log.Info.info),
                    "Yes, Delete log",
                    "No, keep log")) {
                    JMilesDebug.Clear(index);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (log.Expanded) {
                EditorGUILayout.BeginVertical();
                DrawExpandedInfo(log, index);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = backgroundColor;
        }

        private static void DrawExpandedInfo(JMilesDebug.DebugLoggerData log, int index) {
            var gui = new GUIStyle("label") {fontSize = 12};
            if(log.Info.objectOfInfo)
            if (GUILayout.Button(new GUIContent(log.Info.objectOfInfo.ToString()), gui)) {
                Select(log.Info.objectOfInfo);
            }
            EditorGUILayout.TextArea(log.Info.stack);
        }

        private static void DrawIOButtons() {
            if (GUILayout.Button(new GUIContent("Clear all logs", "Warning this WILL delete any and all logs!"))) {
                if (EditorUtility.DisplayDialog(
                    "WARNING: DELETE ALL LOGS",
                    "Warning you are about to delete all the logs from this window, are you sure this is what you want to do?",
                    "Yes, Delete logs",
                    "No, keep logs")) {
                    JMilesDebug.Clear();
                }
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Force Load From (Unsafe)", "Warning this may delete any unsaved logs!"))) {
                JMilesDebug.Load();
            }
            if (GUILayout.Button(new GUIContent("Force Save Log"))) {
                JMilesDebug.Save();
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void Select(Object obj) {
            if(obj != null)
                Selection.activeObject = obj;
        }
    }
}