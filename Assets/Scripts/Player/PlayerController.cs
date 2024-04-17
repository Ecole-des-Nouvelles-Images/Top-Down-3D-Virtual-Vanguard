using System;
using Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public PlayerInput Input;
        public CharacterController Controller;
        [Space]
        public Transform Convoy;
        public Laser Laser;
        public DroneController DroneController;
        public Generator Generator;

        [Header("Settings")]
        public float Speed = 1f;

        [Header("Internal")]
        public LayerMask ModuleLayer;
        public float ModuleDetectionRadius;

        private Vector3 Position => transform.position;
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

        private void Update()
        {
            Controller.Move(_direction);
        }
        
        #region Actions

        #region Convoy

        public void OnMove(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();
            _value = value;
            _direction = Convoy.right * (Mathf.Abs(value.x) >= Math.Abs(value.y) ? value.x : value.y) * Speed * Time.deltaTime;
        }
        
        public void OnEnterModule()
        {
            // ReSharper disable once Unity.PreferNonAllocApi
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

            if (closestModule == null)
            {
                Debug.LogError("No module has been find upon input");
                return;
            }
            
            GameObject module = closestModule.gameObject;
            Debug.Log("Player is inside module: " + module.gameObject.name);
            
            Input.SwitchCurrentActionMap($"{module.gameObject.name}");
        }

        #endregion

        #region Laser

        public void OnRotate(InputValue input)
        {
            Laser.Rotate(input);
        }

        public void OnFire(InputValue input)
        {
            float value = input.Get<float>();
                
            Laser.Fire();
        }

        #endregion

        #region Shield

        

        #endregion

        #region Generator

        public void OnRedirectEnergyLeft()
        {
            Generator.SwitchEnergy(-1);
        }
        
        public void OnRedirectEnergyRight(InputValue input)
        {
            Generator.SwitchEnergy(1);
        }

        #endregion

        #region DroneController

        public void OnMoveDrone(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();
            DroneController.UpdateMove(value);
        }

        #endregion
        
        public void OnExitModule()
        {
            DroneController.ResetDrone();
            Input.SwitchCurrentActionMap("Convoy");
        }

        #endregion
    }
}
