using System;
using Managers;
using UnityEditor;
using UnityEngine;

using Terrain;

namespace Editor
{
    [CustomEditor(typeof(TerrainManager))] [CanEditMultipleObjects]
    public class TerrainManagerEditor: UnityEditor.Editor
    {
        private SerializedProperty _focusMode;
        private SerializedProperty _useFarthermostCamera;
        
        private void OnEnable()
        {
            _focusMode = serializedObject.FindProperty("FocusMode");
            _useFarthermostCamera = serializedObject.FindProperty("UseFarthermostCamera");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDefaultInspector();

            if (GUI.changed)
            {
                CameraManager.Instance.SwitchCameraFocus((FocusMode)_focusMode.intValue, _useFarthermostCamera.boolValue);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
