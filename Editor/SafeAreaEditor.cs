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

        private SerializedProperty applicableOffsetPercentage;
        private SerializedProperty applicableOffsetPixels;
        private SerializedProperty unapplicableOffsetPercentage;
        private SerializedProperty unapplicableOffsetPixels;

        public void OnEnable()
        {
            referenceResolutionSource = serializedObject.FindProperty("source");
            referenceResolution = serializedObject.FindProperty("resolution");
            referenceResolutionWidthAndHeight = serializedObject.FindProperty("resolutionWidthAndHeight");
            applicableOffsetPercentage = serializedObject.FindProperty("applicableOffsetPercentage");
            applicableOffsetPixels = serializedObject.FindProperty("applicableOffsetPixels");
            unapplicableOffsetPercentage = serializedObject.FindProperty("unapplicableOffsetPercentage");
            unapplicableOffsetPixels = serializedObject.FindProperty("unapplicableOffsetPixels");
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

            EditorGUILayout.PropertyField(applicableOffsetPercentage);
            EditorGUILayout.PropertyField(applicableOffsetPixels);
            EditorGUILayout.PropertyField(unapplicableOffsetPercentage);
            EditorGUILayout.PropertyField(unapplicableOffsetPixels);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif