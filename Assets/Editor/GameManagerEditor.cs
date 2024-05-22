using UnityEditor;
using UnityEngine;

using Managers;
using Gameplay;

namespace Editor
{
    [CustomEditor(typeof(GameManager))] [CanEditMultipleObjects]
    public class GameManagerEditor: UnityEditor.Editor
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
