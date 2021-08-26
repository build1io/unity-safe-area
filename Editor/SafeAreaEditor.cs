#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Build1.UnitySafeArea.Editor
{
    [CustomEditor(typeof(SafeArea))]
    [CanEditMultipleObjects]
    public sealed class SafeAreaEditor : UnityEditor.Editor
    {
        private SerializedProperty referenceResolutionSource;
        private SerializedProperty referenceResolution;
        private SerializedProperty referenceResolutionWidthAndHeight;

        private SerializedProperty topApply;
        private SerializedProperty topApplicableOffsetPercentage;
        private SerializedProperty topApplicableOffsetPixels;
        private SerializedProperty topUnapplicableOffsetPercentage;
        private SerializedProperty topUnapplicableOffsetPixels;

        private SerializedProperty bottomApply;
        private SerializedProperty bottomApplicableOffsetPercentage;
        private SerializedProperty bottomApplicableOffsetPixels;
        private SerializedProperty bottomUnapplicableOffsetPercentage;
        private SerializedProperty bottomUnapplicableOffsetPixels;

        private SerializedProperty leftApply;
        private SerializedProperty leftApplicableOffsetPercentage;
        private SerializedProperty leftApplicableOffsetPixels;
        private SerializedProperty leftUnapplicableOffsetPercentage;
        private SerializedProperty leftUnapplicableOffsetPixels;

        private SerializedProperty rightApply;
        private SerializedProperty rightApplicableOffsetPercentage;
        private SerializedProperty rightApplicableOffsetPixels;
        private SerializedProperty rightUnapplicableOffsetPercentage;
        private SerializedProperty rightUnapplicableOffsetPixels;

        public void OnEnable()
        {
            referenceResolutionSource = serializedObject.FindProperty("source");
            referenceResolution = serializedObject.FindProperty("resolution");
            referenceResolutionWidthAndHeight = serializedObject.FindProperty("resolutionWidthAndHeight");

            topApply = serializedObject.FindProperty("topApply");
            topApplicableOffsetPercentage = serializedObject.FindProperty("topApplicableOffsetPercentage");
            topApplicableOffsetPixels = serializedObject.FindProperty("topApplicableOffsetPixels");
            topUnapplicableOffsetPercentage = serializedObject.FindProperty("topUnapplicableOffsetPercentage");
            topUnapplicableOffsetPixels = serializedObject.FindProperty("topUnapplicableOffsetPixels");

            bottomApply = serializedObject.FindProperty("bottomApply");
            bottomApplicableOffsetPercentage = serializedObject.FindProperty("bottomApplicableOffsetPercentage");
            bottomApplicableOffsetPixels = serializedObject.FindProperty("bottomApplicableOffsetPixels");
            bottomUnapplicableOffsetPercentage = serializedObject.FindProperty("bottomUnapplicableOffsetPercentage");
            bottomUnapplicableOffsetPixels = serializedObject.FindProperty("bottomUnapplicableOffsetPixels");

            leftApply = serializedObject.FindProperty("leftApply");
            leftApplicableOffsetPercentage = serializedObject.FindProperty("leftApplicableOffsetPercentage");
            leftApplicableOffsetPixels = serializedObject.FindProperty("leftApplicableOffsetPixels");
            leftUnapplicableOffsetPercentage = serializedObject.FindProperty("leftUnapplicableOffsetPercentage");
            leftUnapplicableOffsetPixels = serializedObject.FindProperty("leftUnapplicableOffsetPixels");

            rightApply = serializedObject.FindProperty("rightApply");
            rightApplicableOffsetPercentage = serializedObject.FindProperty("rightApplicableOffsetPercentage");
            rightApplicableOffsetPixels = serializedObject.FindProperty("rightApplicableOffsetPixels");
            rightUnapplicableOffsetPercentage = serializedObject.FindProperty("rightUnapplicableOffsetPercentage");
            rightUnapplicableOffsetPixels = serializedObject.FindProperty("rightUnapplicableOffsetPixels");
        }

        public override void OnInspectorGUI()
        {
            var safeArea = (SafeArea)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(referenceResolutionSource);

            switch (safeArea.source)
            {
                case ResolutionSource.WidthAndHeight:

                    EditorGUILayout.PropertyField(referenceResolutionWidthAndHeight);

                    if (safeArea.resolutionWidthAndHeight == Vector2.zero)
                        EditorGUILayout.HelpBox("Resolution values not set.", MessageType.Error, false);

                    break;

                case ResolutionSource.Resolution:

                    EditorGUILayout.PropertyField(referenceResolution);

                    if (safeArea.ReferenceResolutionSet)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.Vector2Field("Reference Resolution", safeArea.GetCurrentReferenceResolution());
                        GUI.enabled = true;
                    }

                    EditorGUILayout.HelpBox("Resolution is a ScriptableObject provided with the SafeArea tool.\nCreate an instance via Assets menu and use it for all SafeAreas across your app. Reference resolution management will be centralized.", MessageType.Info, false);

                    if (!safeArea.ReferenceResolutionSet)
                        EditorGUILayout.HelpBox("Resolution not set.", MessageType.Error, false);

                    break;

                case ResolutionSource.CanvasScaler:

                    safeArea.UpdateCanvasScaler();

                    if (safeArea.CanvasScalerFound)
                    {
                        GUI.enabled = false;
                        EditorGUILayout.Vector2Field("Reference Resolution", safeArea.GetCurrentReferenceResolution());
                        GUI.enabled = true;
                    }

                    EditorGUILayout.HelpBox("SafeArea will search for CanvasScaler on itself or parents.\nReference resolution of CanvasScaler will be used.", MessageType.Info, false);
                    if (!safeArea.CanvasScalerFound)
                        EditorGUILayout.HelpBox("CanvasScaler not found.\nIf you're in Prefab Mode, consider adding an Editing Environment.", MessageType.Error, false);

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            EditorGUILayout.PropertyField(topApply, new GUIContent("Apply"));
            if (topApply.boolValue)
            {
                EditorGUILayout.PropertyField(topApplicableOffsetPercentage, new GUIContent("Applicable Offset (%)"));
                EditorGUILayout.PropertyField(topApplicableOffsetPixels, new GUIContent("Applicable Offset (px)"));
                EditorGUILayout.PropertyField(topUnapplicableOffsetPercentage, new GUIContent("Unapplicable Offset (%)"));
                EditorGUILayout.PropertyField(topUnapplicableOffsetPixels, new GUIContent("Unapplicable Offset (px)"));
            }

            EditorGUILayout.PropertyField(bottomApply, new GUIContent("Apply"));
            if (bottomApply.boolValue)
            {
                EditorGUILayout.PropertyField(bottomApplicableOffsetPercentage, new GUIContent("Applicable Offset (%)"));
                EditorGUILayout.PropertyField(bottomApplicableOffsetPixels, new GUIContent("Applicable Offset (px)"));
                EditorGUILayout.PropertyField(bottomUnapplicableOffsetPercentage, new GUIContent("Unapplicable Offset (%)"));
                EditorGUILayout.PropertyField(bottomUnapplicableOffsetPixels, new GUIContent("Unapplicable Offset (px)"));
            }

            EditorGUILayout.PropertyField(leftApply, new GUIContent("Apply"));
            if (leftApply.boolValue)
            {
                EditorGUILayout.PropertyField(leftApplicableOffsetPercentage, new GUIContent("Applicable Offset (%)"));
                EditorGUILayout.PropertyField(leftApplicableOffsetPixels, new GUIContent("Applicable Offset (px)"));
                EditorGUILayout.PropertyField(leftUnapplicableOffsetPercentage, new GUIContent("Unapplicable Offset (%)"));
                EditorGUILayout.PropertyField(leftUnapplicableOffsetPixels, new GUIContent("Unapplicable Offset (px)"));
            }

            EditorGUILayout.PropertyField(rightApply, new GUIContent("Apply"));
            if (rightApply.boolValue)
            {
                EditorGUILayout.PropertyField(rightApplicableOffsetPercentage, new GUIContent("Applicable Offset (%)"));
                EditorGUILayout.PropertyField(rightApplicableOffsetPixels, new GUIContent("Applicable Offset (px)"));
                EditorGUILayout.PropertyField(rightUnapplicableOffsetPercentage, new GUIContent("Unapplicable Offset (%)"));
                EditorGUILayout.PropertyField(rightUnapplicableOffsetPixels, new GUIContent("Unapplicable Offset (px)"));
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Applicable offset will be applied if a side is enabled and safe area padding is > 0.\nUnapplicable offset will be applied if a side is enabled but safe area padding is 0.", MessageType.Info, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif