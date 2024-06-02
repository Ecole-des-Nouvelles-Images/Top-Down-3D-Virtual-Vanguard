using UnityEditor;
using UnityEngine;

using Managers;
using Gameplay;

namespace Editor
{
    [CustomEditor(typeof(GameManager))] [CanEditMultipleObjects]
    public class GameManagerEditor: UnityEditor.Editor
    {
        private SerializedProperty _side;
        private SerializedProperty _useFarthermostCamera;
        
        private void OnEnable()
        {
            _side = serializedObject.FindProperty("Side");
            _useFarthermostCamera = serializedObject.FindProperty("UseFarthermostCamera");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawDefaultInspector();

            if (GUI.changed && CameraManager.Instance)
            {
                CameraManager.Instance.SwitchCameraFocus((Side)_side.intValue, _useFarthermostCamera.boolValue);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
