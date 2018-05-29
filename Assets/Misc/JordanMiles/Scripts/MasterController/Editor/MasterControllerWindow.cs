using System.Collections.Generic;
using Castle.Core.Internal;
using UnityEditor;
using UnityEngine;

namespace JMiles42.Editor {
    public class MasterControllerWindow : Window<MasterControllerWindow> {
        [MenuItem("Tools/JMiles42/Master Control")]
        private static void Init() {
            // Get existing open window or if none, make a new one:
            GetWindow();
            window.titleContent = new GUIContent("Master Control", "JMiles Master Control");

            window.Show();
            window.autoRepaintOnSceneChange = true;
            window.minSize = new Vector2(580, 350);
        }

        private static float healthValue = 5;
        private static float damageValue = 5;
        private static float energyAddValue = 5;
        private static float energyRemoveValue = 5;

        private static Vector2 scrollPos = Vector2.zero;
        private static CharacterBase[] knownCharacterBase = new CharacterBase[1];
        private static Manager Manager;
        private static HumanController HumanController;
        //private static bool followCam = false;
        //private static bool editTarget = false;
        private static bool openAllWindows = false;
        private static Dictionary<CharacterBase, LocalData> toggleDictionary = new Dictionary<CharacterBase, LocalData>(5);
        protected override void OnGUI() { DrawMainContents(); }

        protected override void Update() { Repaint(); }

        private static void GetPlayers() {
            Manager = FindObjectOfType<Manager>();
            HumanController = FindObjectOfType<HumanController>();
            if (!Manager) knownCharacterBase = FindObjectsOfType<CharacterBase>();
        }

        private static void DrawMainContents() {
            if (window == null) GetWindow();
            if (!Manager) GetPlayers();
            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.magenta;

            EditorGUILayout.BeginHorizontal("box", GUILayout.Height(32));
            DrawTitle();
            DrawToggle();
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = backgroundColor;
            EditorGUILayout.Space();

            DrawFloats();

            if (!Application.isPlaying) {
                GUI.backgroundColor = new EditorColour(Color.red, 0.5f);//EditorGUIUtility.isProSkin ? Color.red : new Color(0.6f, 0.1f, 0.1f, 0.8f);
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, "box");

            EditorGUILayout.BeginVertical();
            if (!Manager) {
                foreach (var aiBase in knownCharacterBase) {
                    DrawPlayerLoop(backgroundColor, aiBase);
                }
            }
            else {
                foreach (var aiBase in Manager.CharacterBases) {
                    DrawPlayerLoop(backgroundColor, aiBase);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUI.backgroundColor = backgroundColor;
            if (!Application.isPlaying) {
                EditorGUILayout.HelpBox(
                    "These buttons are disabled while the game is not running, to prevent unwanted calls during edit time.!",
                    MessageType.Info);
            }
        }

        private static void DrawPlayerLoop(Color backgroundColor, CharacterBase aiBase) {
            if (Selection.activeGameObject == aiBase.gameObject) {
                GUI.backgroundColor = new EditorColour(Color.yellow, 0.5f);//EditorGUIUtility.isProSkin ? Color.yellow : Color.yellow * 0.5f;
            }
            else {
                if (!Application.isPlaying)
                    GUI.backgroundColor = new EditorColour(Color.red, 0.6f);//EditorGUIUtility.isProSkin ? Color.red : new Color(0.6f, 0.1f, 0.1f, 0.8f);
                else GUI.backgroundColor = backgroundColor;
            }

            EditorGUILayout.BeginHorizontal("box", GUILayout.Height(32));
            DrawPlayer(aiBase);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawTitle() { DrawTitle("JMiles Master Control Panel"); }

        private static void DrawPlayer(CharacterBase aiBase) {
            var health = aiBase.GetComponent<Health>();
            var energy = aiBase.GetComponent<Energy>();

            var backgroundColor = GUI.backgroundColor;

            EditorGUILayout.BeginVertical();//v1
            EditorGUILayout.BeginHorizontal();//h1
            LocalData localData;

            {
                toggleDictionary.TryGetValue(aiBase, out localData);
                if (localData == null) {
                    localData = new LocalData();
                }
                localData.enabled = EditorGUILayout.Foldout(localData.enabled, "", true);
                var gui = new GUIStyle("Label") {fontSize = 18};
                if (GUILayout.Button(
                    new GUIContent(string.Format("{0}", aiBase.characterName), string.Format("{0}", aiBase)),
                    gui,
                    GUILayout.Width(window.minSize.y / 2))) {
                    Select(aiBase);
                }
            }

            EditorGUILayout.Space();
            DrawBars(health, energy);//Opens And closes all

            EditorGUILayout.EndHorizontal();//h0

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);

            //if (followCam) {
            //	if (GUILayout.Button(new GUIContent("Follow Character", "Set the camera to follow Character."), GUILayout.Height(18 * 2), GUILayout.Width(128))) {
            //		TakeControl(aiBase);
            //	}
            //}
            //else {
            //	EditorGUILayout.BeginHorizontal(GUILayout.Height(18 * 2), GUILayout.Width(128));
            //	EditorGUILayout.HelpBox("WARNING: No follow camera found.", MessageType.Warning);
            //	EditorGUILayout.EndHorizontal();
            //}

            if (localData.enabled || openAllWindows) {
                EditorGUILayout.BeginHorizontal();//h1
                HumanControlData(aiBase);
                AbilityData(aiBase);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                HealthData(health, backgroundColor);
                EnergyData(energy, backgroundColor);
                TargetData(aiBase, ref localData);
            }
            EditorGUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();
            toggleDictionary[aiBase] = localData;
            GUI.backgroundColor = backgroundColor;
        }

        private static void HumanControlData(CharacterBase aiBase) {
            if (HumanController) {
                if (HumanController.TargetModel == aiBase) {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Width(128));
                    EditorGUILayout.LabelField("Under Control", GUILayout.Width(128));
                    EditorGUILayout.EndHorizontal();
                }
                else {
                    if (GUILayout.Button(new GUIContent("Take Control", "Set this object to be controled by, you!"), GUILayout.Width(128))) {
                        TakeControl(aiBase);
                    }
                }
            }
            else {
                EditorGUILayout.BeginHorizontal(GUILayout.Width(128));//h2
                EditorGUILayout.HelpBox("WARNING: No Human Controller found.", MessageType.Warning);
                EditorGUILayout.EndHorizontal();//h1
            }
        }

        private static void AbilityData(CharacterBase aiBase) {
            //EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            AbilityButton(aiBase, "Ability1");
            AbilityButton(aiBase, "Ability2");
            AbilityButton(aiBase, "Ability3");
            AbilityButton(aiBase, "Ability4");
            AbilityButton(aiBase, "Ability5");
            EditorGUILayout.EndHorizontal();
            //EditorGUILayout.EndVertical();
        }

        private static void EnergyData(Energy energy, Color backgroundColor) {
            if (energy) {
                EnergyData(energy);
            }
            else {
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.cyan;
                EditorGUILayout.HelpBox(
                    "WARNING: No Energy Component found.\nThis means it is not attached to the same object as this,\nand does not know how to find it.",
                    MessageType.Warning);

                GUI.backgroundColor = backgroundColor;
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void HealthData(Health health, Color backgroundColor) {
            if (health) {
                EditorGUILayout.BeginHorizontal();
                string hstr = string.Format(("Add {0} Health"), healthValue);
                if (GUILayout.Button(new GUIContent(hstr, hstr))) {
                    health.Change(healthValue,(GameObject)null);
                }
                hstr = "Heal to full health";
                if (GUILayout.Button(new GUIContent(hstr, hstr))) {
                    health.Change(float.MaxValue, (GameObject) null);
                }

                string dstr = string.Format(("Deal {0} Damage"), damageValue);
                if (GUILayout.Button(new GUIContent(dstr, dstr))) {
                    health.Change(-damageValue, (GameObject) null);
                }
                dstr = "Kill";
                if (GUILayout.Button(new GUIContent(dstr, dstr))) {
                    health.Change(float.MinValue, (GameObject) null);
                }
                EditorGUILayout.EndHorizontal();
            }
            else {
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = Color.cyan;
                EditorGUILayout.HelpBox(
                    "WARNING: No Health Component found.\nThis means it is not attached to the same object as this,\nand does not know how to find it.",
                    MessageType.Warning);

                GUI.backgroundColor = backgroundColor;
                EditorGUILayout.EndHorizontal();
            }
        }

        private static void EnergyData(Energy energy) {
            EditorGUILayout.BeginHorizontal();
            string estr = string.Format(("Add {0} Energy"), energyAddValue);
            if (GUILayout.Button(new GUIContent(estr, estr))) {
                energy.Change(energyAddValue);
            }
            estr = string.Format(("Remove {0} Energy"), energyRemoveValue);
            if (GUILayout.Button(new GUIContent(estr, estr))) {
                energy.Change(-energyRemoveValue);
            }
            estr = "Set to max energy";
            if (GUILayout.Button(new GUIContent(estr, estr))) {
                energy.Change(float.MaxValue);
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void TargetData(CharacterBase aiBase, ref LocalData data) {
            data.editTargetData = EditorGUILayout.BeginToggleGroup(
                new GUIContent("EDIT TARGET DATA AT OWN RISK!", "As this is just setting the target property"),
                data.editTargetData);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Target", "Target Pos"), GUILayout.Width(32 + 16));
            aiBase.Target = EditorHelpers.DrawVector3("", aiBase.Target, Vector3.zero, aiBase);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
        }

        private static void Select(CharacterBase aiBase) { Selection.activeGameObject = aiBase.gameObject; }

        private static void DrawBars(Health health, Energy energy) {
            EditorGUILayout.BeginHorizontal();
            var rH = EditorGUILayout.BeginVertical();

            var backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            if (health) {
                EditorGUI.ProgressBar(rH, health.Amount / health.maxAmount, string.Format("Health {0}", health.Amount));
            }
            else {
                GUI.backgroundColor = Color.cyan;
                EditorGUI.ProgressBar(rH, 1f, "Health Component Not Found");
            }
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();
            var rE = EditorGUILayout.BeginVertical();

            GUI.backgroundColor = Color.blue;
            if (energy) {
                EditorGUI.ProgressBar(rE, energy.Amount / energy.MaxEnergy, string.Format("Energy {0}", energy.Amount));
            }
            else {
                GUI.backgroundColor = Color.cyan;
                EditorGUI.ProgressBar(rE, 1f, "Energy Component Not Found");
            }
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();
            GUI.backgroundColor = backgroundColor;
            EditorGUILayout.EndHorizontal();
        }

        private static void TakeControl(CharacterBase aiBase) {
            if (HumanController) {
                HumanController.Possess(aiBase);
            }
            else {
                Debug.LogWarning("WARNING!!! Human Controller, Not found!");
            }
        }

        private static void DrawFloats() {
            EditorGUILayout.BeginHorizontal();
            energyAddValue = EditorGUILayout.FloatField(new GUIContent("Energy Add Amount", "Amount of energy to add"), energyAddValue);
            energyRemoveValue = EditorGUILayout.FloatField(new GUIContent("Energy Remove Amount", "Amount of energy to Remove"), energyRemoveValue);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            healthValue = EditorGUILayout.FloatField(new GUIContent("Health Amount", "Amount of health to add"), healthValue);
            damageValue = EditorGUILayout.FloatField(new GUIContent("Damage Amount", "Amount of damage to deal"), damageValue);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawToggle() {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            openAllWindows = EditorGUILayout.Toggle(new GUIContent("Force Open Tabs open", "Forces all Character Tabs Open"), openAllWindows);
            EditorGUILayout.EndVertical();
        }

        private static void AbilityButton(CharacterBase aiBase, string abilityMethodName) {
            //if ((aiBase.GetType().GetMethod(abilityMethodName).IsMethodFromType<CharacterBase>()))
            var aiBaseMethod = aiBase.GetType().GetMethod(abilityMethodName);
            if (!(aiBaseMethod.IsOverride())) {
                //	EditorGUI.BeginDisabledGroup(true);
                //	GUILayout.Button(new GUIContent(string.Format("{0} Not Override", abilityMethodName)));
                //	EditorGUI.EndDisabledGroup();
            }
            else if (aiBaseMethod.GetAttribute<HasNoAbility>() != null) {
                //	EditorGUI.BeginDisabledGroup(true);
                //	GUILayout.Button(new GUIContent(string.Format("Has no {0}", abilityMethodName)));
                //	EditorGUI.EndDisabledGroup();
            }
            else {
                if (GUILayout.Button(new GUIContent(abilityMethodName, string.Format("Activate {0}", abilityMethodName)))) {
                    aiBaseMethod.Invoke(aiBase, null);
                }
            }
        }

        private static bool DoesImplimentsAnyAbilities(CharacterBase aiBase) {
            var t = aiBase.GetType();
            if (t.GetMethod("Ability1").IsOverride() &&
                t.GetMethod("Ability2").IsOverride() &&
                t.GetMethod("Ability3").IsOverride() &&
                t.GetMethod("Ability4").IsOverride() &&
                t.GetMethod("Ability5").IsOverride()) {
                return true;
            }
            return false;
        }

        private static bool DoesNotHaveAbilities(CharacterBase aiBase) {
            var t = aiBase.GetType();
            if (t.GetMethod("Ability1").GetAttribute<HasNoAbility>() != null &&
                t.GetMethod("Ability2").GetAttribute<HasNoAbility>() != null &&
                t.GetMethod("Ability3").GetAttribute<HasNoAbility>() != null &&
                t.GetMethod("Ability4").GetAttribute<HasNoAbility>() != null &&
                t.GetMethod("Ability5").GetAttribute<HasNoAbility>() != null) {
                return true;
            }
            return false;
        }

        private class LocalData {
            public bool enabled;
            public bool editTargetData;

            public LocalData() {
                enabled = false;
                editTargetData = false;
            }
        }
    }
}