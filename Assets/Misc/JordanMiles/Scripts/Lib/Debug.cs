using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JMiles42 {
    public static class JMilesDebug {
        public static string FILENAME = "Log.data";
        public static string FILEPATH = "Assets/Misc/JordanMiles/EditorData";
        public static string FILEPATHNAME = FILEPATH + "/" + FILENAME;

        static JMilesDebug() {
            LoadAdd();
            //Application.logMessageReceived -= ApplicationOnLogMessageReceived;
            //Application.logMessageReceived += ApplicationOnLogMessageReceived;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded() {
            //Application.logMessageReceived -= ApplicationOnLogMessageReceived;
            //Application.logMessageReceived += ApplicationOnLogMessageReceived;
            Save();
        }

        private static void ApplicationOnLogMessageReceived(string condition, string stackTrace, LogType type) {
            debugData.AddLog(new DebugLoggerData.DebugInfo(type, condition, null, stackTrace));
        }

        public static DebugData debugData = new DebugData();

        public static void Load() {
            if (File.Exists(FILEPATHNAME)) {
                debugData = JsonUtility.FromJson<DebugData>(File.ReadAllText(FILEPATHNAME));
            }
            else {
                debugData = new DebugData();
                Save();
                AssetDatabase.Refresh();
            }
        }

        public static void LoadAdd() {
            if (File.Exists(FILEPATHNAME)) {
                var tempData = debugData;
                debugData = JsonUtility.FromJson<DebugData>(File.ReadAllText(FILEPATHNAME));
                debugData.debugLog.AddRange(tempData.debugLog);
            }
            else {
                debugData = new DebugData();
                Save();
                AssetDatabase.Refresh();
            }
        }

        public static void Save() {
            if (Application.isPlaying) return;
            using (var outfile = new StreamWriter(FILEPATHNAME)) {
                outfile.WriteLine(JsonUtility.ToJson(debugData, true));
            }//File written
        }

        public static void Clear() {
            debugData.debugLog.Clear();
            Save();
        }

        public static void Clear(int index) {
            debugData.debugLog.RemoveAt(index);
            Save();
        }

        public static void Log(object obj, Object unityObj = null) {
            debugData.AddLog(new DebugLoggerData.DebugInfo(LogType.Log, obj.ToString(), unityObj, StackTraceUtility.ExtractStackTrace()));
            Save();
            //return;
            //
            //if (!unityObj) {
            //    Debug.Log("[JMiles] " + obj);
            //}
            //else {
            //    Debug.Log("[JMiles] " + obj, unityObj);
            //}
        }

        public static void LogWarning(object obj, Object unityObj = null) {
            debugData.AddLog(new DebugLoggerData.DebugInfo(LogType.Warning, obj.ToString(), unityObj, Environment.StackTrace));
            Save();
            //return;
            //
            //if (!unityObj) {
            //    Debug.LogWarning("[JMiles] " + obj);
            //}
            //else {
            //    Debug.LogWarning("[JMiles] " + obj, unityObj);
            //}
        }

        public static void LogError(object obj, Object unityObj = null) {
            debugData.AddLog(new DebugLoggerData.DebugInfo(LogType.Error, obj.ToString(), unityObj, Environment.StackTrace));
            Save();
            //return;
            //
            //if (!unityObj) {
            //    Debug.LogError("[JMiles] " + obj);
            //}
            //else {
            //    Debug.LogError("[JMiles] " + obj, unityObj);
            //}
        }

        public static void Log(DebugLoggerData.DebugInfo info) {
            debugData.AddLog(info);
            Save();
        }

        public class DebugData {
            public List<DebugLoggerData> debugLog = new List<DebugLoggerData>(10);

            public void AddLog(DebugLoggerData.DebugInfo debugInfo) {
                foreach (var t in debugLog) {
                    if (DebugLoggerData.DebugInfo.Compair(t.Info, debugInfo)) {
                        t.Count++;
                        return;
                    }
                }
                debugLog.Add(new DebugLoggerData(debugInfo));
            }
        }

        [Serializable]
        public class DebugLoggerData {
            public DebugInfo Info;
            public bool Expanded;
            public int Count;

            public DebugLoggerData(DebugInfo _info) {
                Info = _info;
                Expanded = false;
                Count = 1;
            }

            public DebugLoggerData(DebugInfo _info, int _count) {
                Info = _info;
                Expanded = false;
                Count = _count;
            }

            public DebugLoggerData(DebugInfo _info, bool _expanded) {
                Info = _info;
                Expanded = _expanded;
                Count = 1;
            }

            public DebugLoggerData(DebugInfo _info, bool _expanded, int _count) {
                Info = _info;
                Expanded = _expanded;
                Count = _count;
            }

            [Serializable]
            public class DebugInfo {
                public LogType rating;
                public Object objectOfInfo;
                public string info;
                public string stack;

                public DebugInfo() {
                    rating = LogType.Log;
                    objectOfInfo = null;
                    info = "New Log";
                    stack = "";
                }

                public DebugInfo(LogType rate, string _info, Object objOfInfo, string _stack) {
                    rating = rate;
                    objectOfInfo = objOfInfo;
                    info = _info;
                    stack = _stack;
                }

                public static bool Compair(DebugInfo a, DebugInfo b) {
                    return (a.rating == b.rating) && (a.objectOfInfo == b.objectOfInfo) && (a.info == b.info) && (a.stack == b.stack);
                }

                public override bool Equals(System.Object obj) {
                    // If parameter is null return false.
                    if (obj == null) {
                        return false;
                    }

                    // If parameter cannot be cast to Point return false.
                    var p = obj as DebugInfo;
                    if ((System.Object) p == null) {
                        return false;
                    }

                    // Return true if the fields match:
                    return (rating == p.rating) && (objectOfInfo == p.objectOfInfo) && (info == p.info) && (stack == p.stack);
                }

                public bool Equals(DebugInfo p) {
                    // If parameter is null return false:
                    if ((object) p == null) {
                        return false;
                    }

                    // Return true if the fields match:
                    return (rating == p.rating) && (objectOfInfo == p.objectOfInfo) && (info == p.info) && (stack == p.stack);
                }

                public override int GetHashCode() {
                    unchecked {
                        var hashCode = (int) rating;
                        hashCode = (hashCode * 397) ^ (objectOfInfo != null ? objectOfInfo.GetHashCode() : 0);
                        hashCode = (hashCode * 397) ^ (info != null ? info.GetHashCode() : 0);
                        hashCode = (hashCode * 397) ^ (stack != null ? stack.GetHashCode() : 0);
                        return hashCode;
                    }
                }
            }
        }
    }
}