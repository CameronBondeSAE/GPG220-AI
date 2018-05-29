﻿using UnityEditor;using UnityEngine;namespace JMiles42.Editor.Editors {    [CustomEditor(typeof(Energy)),CanEditMultipleObjects]    public class EnergyEditor : UnityEditor.Editor {        public override void OnInspectorGUI() {            base.DrawDefaultInspector();            var energy = target as Energy;            var backgroundColor = GUI.backgroundColor;            GUI.backgroundColor = Color.blue;            var rE = EditorGUILayout.BeginVertical();            EditorGUI.ProgressBar(rE, energy.Amount / energy.MaxEnergy, "Energy");            GUILayout.Space(18);            EditorGUILayout.EndVertical();            GUI.backgroundColor = backgroundColor;            rE = EditorGUILayout.BeginVertical();            if (GUI.Button(rE, "Set To Max Energy")) {                energy.Change(energy.MaxEnergy);            }            GUILayout.Space(18);            EditorGUILayout.EndVertical();        }    }}