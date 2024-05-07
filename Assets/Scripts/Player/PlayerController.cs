using System;
using Modules;
using Script.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public static int PlayerNumber = 0;
        
        public int ID;
        public float MoveSpeed;
        
        private CharacterController _controller;
        private PlayerInputActions _actions;

        private Module _operatingModule;
        public bool IsBusy { get; set; }
        
        private Vector3 _motion;

        private void Awake()
        {
            ID = ++PlayerNumber;
            _controller = GetComponent<CharacterController>();
            _actions = new PlayerInputActions();
        }

        private void Update()
        {
            if (!IsBusy)
                _controller.Move(_motion);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Module")) return;

            _operatingModule = other.GetComponent<Module>();
        }

        #region Actions

        public void OnMove(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();

            _motion = transform.right * (Mathf.Abs(value.x) >= Math.Abs(value.y) ? value.x : value.y) * MoveSpeed * Time.deltaTime;
        }

        public void OnModuleEnter()
        {
            if (_operatingModule == null) return;

            Debug.Log($"Entering module {_operatingModule.name}");
            _operatingModule.EnterModule(this);
        }

        public void OnModuleExit()
        {
            if (_operatingModule == null) return;
            
            Debug.Log($"Exiting module {_operatingModule.name}");
            _operatingModule.ExitModule(this);
        }

        public void OnModuleOperate()
        {
            if (_operatingModule == null || !IsBusy) return;
            
            _operatingModule.Operate();
        }

        public void OnModuleInteract()
        {
            if (_operatingModule == null || !IsBusy) return;
        }

        public void OnModuleAim(InputValue input)
        {
            if (_operatingModule == null || !IsBusy) return;
            
            _operatingModule.Aim(input);
        }

        #endregion
    }
}
