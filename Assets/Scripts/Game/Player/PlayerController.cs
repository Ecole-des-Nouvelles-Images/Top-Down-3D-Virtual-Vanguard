using System;
using Game.Convoy;
using Game.Convoy.Drones;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        public float MoveSpeed;
        
        public bool IsBusy { get; set; }
        
        private Rigidbody _rigidbody;
        private Module _operatingModule;
        private Drone _operatingDrone;
        private TMP_Text _idPanel;
        private Image _idPanelArrow;
        
        public int PlayerID { get; private set; }
        public Color PlayerColor { get; private set; }
        private Vector2 _inputValue;
        private Vector3 _referenceMoveAxis;

        private void Awake()
        {
            PlayerID = PlayerManager.Instance.PlayerNumber;
            _rigidbody = GetComponent<Rigidbody>();
            
            SetupColorAndUI();
        }

        private void Start()
        {
            transform.position = ConvoyManager.Modules[0].transform.position;
        }

        private void Update()
        {
            if (!IsBusy)
                Move();
            if (IsBusy && _operatingDrone && _operatingDrone.Active)
                _operatingDrone.Move(_inputValue);
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

            _inputValue = value;
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
            
            _operatingModule.Operate(this);
        }

        public void OnModuleInteract()
        {
            if (_operatingModule == null || !IsBusy) return;
            
            _operatingModule.Interact(this);
        }

        public void OnModuleAim(InputValue input)
        {
            if (_operatingModule == null || !IsBusy) return;
            
            _operatingModule.Aim(input);
        }

        #endregion

        public void Move()
        {
            Vector3 moveAxis = transform.right + transform.forward;
            Vector3 motion = moveAxis * ((Mathf.Abs(_inputValue.x) >= Math.Abs(_inputValue.y) ? _inputValue.x : _inputValue.y) * MoveSpeed * Time.deltaTime);
            _rigidbody.AddForce(motion, ForceMode.Acceleration);
        }

        public void AssignDrone(Drone drone)
        {
            _operatingDrone = drone;
        }

        public void UnassignDrone()
        {
            _operatingDrone = null;
        }

        private void SetupColorAndUI()
        {
            switch (PlayerID)
            {
                case 1:
                    PlayerColor = new Color(197 / 255f, 32 / 255f, 38 / 255f);
                    break;
                case 2:
                    PlayerColor = new Color(56 / 255f, 77 / 255f, 161 / 255f);
                    break;
                case 3:
                    PlayerColor = new Color(246 / 255f, 237 / 255f, 97 / 255f);
                    break;
                case 4:
                    PlayerColor = new Color(102 / 255f, 71 / 255f, 156 / 255f);
                    break;
            }
            
            _idPanel = GetComponentInChildren<TMP_Text>();
            _idPanel.text = "J" + PlayerID;
            _idPanel.color = PlayerColor;
            _idPanelArrow = GetComponentInChildren<Image>();
            _idPanelArrow.color = PlayerColor;
        }
    }
}
