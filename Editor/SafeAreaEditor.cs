#if UNITY_EDITOR

using System;
using UnityEditor;

namespace Build1.PostMVC.Extensions.Unity.Components
{
    [CustomEditor(typeof(SafeArea))]
    [CanEditMultipleObjects]
    public sealed class SafeAreaEditor: UnityEditor.Editor
    {
        private SerializedProperty referenceResolutionSource;
        private SerializedProperty referenceResolution;
        private SerializedProperty referenceResolutionVector;
        
        private SerializedProperty applicableOffsetPercentage;
        private SerializedProperty applicableOffsetPixels;
        private SerializedProperty unapplicableOffsetPercentage;
        private SerializedProperty unapplicableOffsetPixels;
    
        public void OnEnable()
        {
            referenceResolutionSource = serializedObject.FindProperty("source");
            referenceResolution = serializedObject.FindProperty("resolution");
            referenceResolutionVector = serializedObject.FindProperty("widthAndHeight");
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
                case SafeAreaReferenceResolutionSource.WidthAndHeight:
                    EditorGUILayout.PropertyField(referenceResolutionVector);
                    break;
                
                case SafeAreaReferenceResolutionSource.Resolution:
                    EditorGUILayout.PropertyField(referenceResolution);
                    EditorGUILayout.HelpBox("Resolution is a ScriptableObject provided with the SafeArea package.\nCreate an instance and use it for all SafeAreas across your app.\nReference resolution changing will be centralized.", MessageType.Info, false);
                    break;
                
                case SafeAreaReferenceResolutionSource.CanvasScaler:
                    EditorGUILayout.HelpBox("SafeArea will search for CanvasScaler on itself or parents.\nReference resolution of CanvasScaler will be used.", MessageType.Info, false);
                    
                    
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