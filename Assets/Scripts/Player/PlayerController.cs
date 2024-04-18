using System;
using System.Text.RegularExpressions;
using Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerInfo))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public PlayerInput Input;
        public CharacterController Controller;

        [Header("Settings")]
        public float Speed = 1f;

        [Header("Internal")]
        public LayerMask ModuleLayer;
        public float ModuleDetectionRadius;

        private PlayerInfo _info;
        private MeshRenderer _renderer;

        private Transform _convoy;
        private Module _currentActiveModule;

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

        private void Awake()
        {
            _info = GetComponent<PlayerInfo>();
            _renderer = GetComponent<MeshRenderer>();

            _renderer.material = _info.Material;
            _info.UIPositionIcon.color = _info.Material.color;
            _info.UIPositionText.color = _info.Material.color;

            _convoy = GameObject.Find("Convoy").transform;
        }

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
            _direction = _convoy.right * (Mathf.Abs(value.x) >= Math.Abs(value.y) ? value.x : value.y) * Speed * Time.deltaTime;
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
                Debug.LogError("No module object has been found upon input");
                return;
            }
            
            _currentActiveModule = closestModule.gameObject.GetComponent<Module>();
            Input.SwitchCurrentActionMap($"{Regex.Replace(_currentActiveModule.gameObject.name, @"\d", "")}");
            Debug.Log("Player is inside module: " + _currentActiveModule.gameObject.name);
        }

        #endregion

        #region Laser

        public void OnRotate(InputValue input)
        {
            if (_currentActiveModule == null || _currentActiveModule.GetType() != typeof(Laser))
                throw new Exception("Unexpected error: no module found or module type mismatch");
            
            Laser laser = _currentActiveModule as Laser;
            laser.Rotate(input);
        }

        public void OnFire()
        {
            if (_currentActiveModule == null || _currentActiveModule.GetType() != typeof(Laser))
                throw new Exception("Unexpected error: no module found or module type mismatch");
            
            Laser laser = _currentActiveModule as Laser;
            laser.Fire();
        }

        #endregion

        #region Shield

        public void OnSwitchPower()
        {
            if (_currentActiveModule == null || _currentActiveModule.GetType() != typeof(Shield))
                throw new Exception("Unexpected error: no module found or module type mismatch");
            
            Shield shield = _currentActiveModule as Shield;
            shield.SwitchPower();
        }

        public void OnSwitchPolarity()
        {
            if (_currentActiveModule == null || _currentActiveModule.GetType() != typeof(Shield))
                throw new Exception("Unexpected error: no module found or module type mismatch");
            
            Shield shield = _currentActiveModule as Shield;
            shield.SwitchPolarity();
        }

        #endregion

        #region Generator

        public void OnRedirectEnergyLeft()
        {
            if (_currentActiveModule == null || _currentActiveModule.GetType() != typeof(Generator))
                throw new Exception("Unexpected error: no module found or module type mismatch");
            
            Generator generator = _currentActiveModule as Generator;
            generator.SwitchEnergy(-1);
        }
        
        public void OnRedirectEnergyRight(InputValue input)
        {            
            if (_currentActiveModule == null || _currentActiveModule.GetType() != typeof(Generator))
                throw new Exception("Unexpected error: no module found or module type mismatch");
            
            Generator generator = _currentActiveModule as Generator;
            generator.SwitchEnergy(1);
        }

        #endregion

        #region DroneController

        public void OnMoveDrone(InputValue input)
        {
            if (_currentActiveModule == null || _currentActiveModule.GetType() != typeof(DroneController))
                throw new Exception("Unexpected error: no module found or module type mismatch");
            
            DroneController droneController = _currentActiveModule as DroneController;
            Vector2 value = input.Get<Vector2>();
            droneController.UpdateMove(value);
        }

        #endregion
        
        public void OnExitModule()
        {
            Input.SwitchCurrentActionMap("Convoy");
        }

        #endregion
    }
}
