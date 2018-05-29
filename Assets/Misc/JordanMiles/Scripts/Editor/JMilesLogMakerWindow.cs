using UnityEditor;
using UnityEngine;

namespace JMiles42.Editor {
	public class JMilesLogMakerWindow : Window<JMilesLogMakerWindow> {
		[MenuItem("Tools/JMiles42/Log Maker")]
		private static void Init() {
			// Get existing open window or if none, make a new one:
			GetWindow();
			window.titleContent = new GUIContent("JMiles LogM");

			window.Show();
			window.autoRepaintOnSceneChange = true;
			window.minSize = new Vector2(580, 125);
		}

		private JMilesDebug.DebugLoggerData.DebugInfo info = new JMilesDebug.DebugLoggerData.DebugInfo();


		protected override void OnGUI() {
			var backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.magenta;
			EditorGUILayout.BeginHorizontal("box", GUILayout.Height(32));
			DrawTitle();
			EditorGUILayout.EndHorizontal();
			GUI.backgroundColor = backgroundColor;
			DrawButtons();
		}

		private void DrawTitle() {
			DrawTitle("JMiles Log Maker Window");
		}

		private void DrawButtons() {
			info.info = EditorGUILayout.TextField(new GUIContent("Log"), info.info,GUILayout.Height(18));
			info.rating = (LogType) EditorGUILayout.EnumPopup(new GUIContent("Log Kind"), info.rating);
			if(GUILayout.Button(new GUIContent("Submit Log"))) {
				info.objectOfInfo = this;
				info.stack = StackTraceUtility.ExtractStackTrace();
				JMilesDebug.Log(info);
				info = new JMilesDebug.DebugLoggerData.DebugInfo();
				Repaint();
			}
		}
	}
}