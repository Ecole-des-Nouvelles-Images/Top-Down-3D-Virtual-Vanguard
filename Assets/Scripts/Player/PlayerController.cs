using System;
using Convoy;
using Script.Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public static int PlayerNumber = 0;
        
        public bool IsBusy { get; set; }
        public float MoveSpeed;
        
        private CharacterController _controller;
        private Module _operatingModule;
        private TMP_Text _idPanel;
        
        private int PlayerID { get; set; }
        private Vector3 _motion;

        private void Awake()
        {
            PlayerID = ++PlayerNumber;
            _controller = GetComponent<CharacterController>();
            _idPanel = GetComponentInChildren<TMP_Text>();
            _idPanel.text = "J" + PlayerID;
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

            _operatingModule.EnterModule(this);
        }

        public void OnModuleExit()
        {
            if (_operatingModule == null) return;
            
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
            
            _operatingModule.Interact();
        }

        public void OnModuleAim(InputValue input)
        {
            if (_operatingModule == null || !IsBusy) return;
            
            _operatingModule.Aim(input);
        }

        #endregion
    }
}
