using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public Transform Convoy;
        public PlayerInput Input;
        public CharacterController Controller;

        [Header("Settings")]
        public float Speed = 1f;

        [Header("Internal")]
        public LayerMask ModuleLayer;
        public float ModuleDetectionRadius;

        private InputActionMap _convoy;
        private InputActionMap _laser;
        private InputActionMap _shield;
        private InputActionMap _generator;
        private InputActionMap _drones;

        private Vector3 Position {
            get => transform.position;
            set => transform.position = value;
        } 
        private Vector3 _direction;

        #region Debug

        private Vector2 _value;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Position, ModuleDetectionRadius);
            
            Vector3 labelPosition = transform.position + Vector3.up * 1.5f;
            Handles.Label(labelPosition, $"({_value.x:F3}, {_value.y:F3})");
        }

        #endregion

        private void Awake()
        {
        }

        private void Update()
        {
            Controller.Move(_direction);
        }


        #region Actions

        public void OnMove(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();
            _value = value;
            _direction = Convoy.right * (Mathf.Abs(value.x) >= Math.Abs(value.y) ? value.x : value.y) * Speed * Time.deltaTime;
        }
        
        public void OnEnterModule()
        {
            Collider[] colliders = Physics.OverlapSphere(Position, ModuleDetectionRadius, ModuleLayer);

            Collider closestModule = null;
            float closestDistance = float.MaxValue;

            foreach (Collider contact in colliders)
            {
                if (contact.CompareTag("Module"))
                {
                    Vector3 closestPoint = contact.ClosestPoint(transform.position);

                    float distance = Vector3.Distance(Position, closestPoint);
                    if (distance < closestDistance)
                    {
                        closestModule = contact;
                        closestDistance = distance;
                    }
                }
            }

            if (closestModule == null) return;
            
            GameObject module = closestModule.gameObject;
            Debug.Log("Player is inside module: " + module.gameObject.name);
            
            Input.SwitchCurrentActionMap($"{module.gameObject.name}");
        }

        public void OnExitModule()
        {
            Input.SwitchCurrentActionMap("Convoy");
        }

        #endregion
    }
}
