﻿using System;
using System.Linq;
using Game.Convoy.Modules;
using Game.Player;
using Game.POIs;
using UnityEngine;

namespace Game.Convoy.Drones
{
    [RequireComponent(typeof(Rigidbody))]
    public class Drone: MonoBehaviour
    {
        public static int TotalDroneBuilt;
        
        public int ID { get; private set; }
        public int ArmorPlates { get; set; }
        
        public DroneController DroneController { get; set; }
        public Anchor AssignedAnchor { get; set; }
        public bool Active { get; set; }
        
        public bool Operating {
            get => _operating;
            set {
                _operating = value;
                if (_interacting && _operating)
                    _interacting = false;
                if( _operating==false)SetMiningVFX(value);
            }
        }
        public bool Interacting {
            get => _interacting;
            set {
                _interacting = value;
                if (_operating && _interacting)
                    _operating = false;
            }
        }
        
        public PlayerController Pilot { get; set; }
        public bool IsRegistered => Pilot != null;
        public bool IsTargetable => true;
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;

        [Space(5), Header("VFX")]
        [SerializeField] private ParticleSystem _psMinig;

        private Rigidbody _rigidbody;
        private bool _operating;
        private bool _interacting;
        private POI _nearestPOI;
        
        private float _moveSpeed;
        private float _miningSpeed;
        private float _accumulatedMinedAmount;

        #region Debug

        [Header("Debug")]
        public bool DebugRangeCollider;

        #endregion

        private void Awake()
        {
            ID = ++TotalDroneBuilt;
            _rigidbody = GetComponent<Rigidbody>();
            _moveSpeed = DroneController.DroneMoveSpeed;
            _miningSpeed = DroneController.DroneMiningSpeed;
        }

        private void Update()
        {
            if (!_nearestPOI) return;
            
            if (Operating)
            {
                Operate();
            }
            else if (Interacting)
            {
                Interact();
            }
        }

        private void OnDisable()
        {
            if (AssignedAnchor) AssignedAnchor = null;
            Pilot?.UnassignDrone();
            Pilot = null;
            DroneController?.UnregisterDrone(this);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("POI")) return;

            _nearestPOI = other.GetComponent<POI>();
            Debug.Log($"Drone #{ID} near of POI: {_nearestPOI.name}");
        }

        private void OnTriggerExit(Collider other)
        {
            _nearestPOI = null;
        }

        public void RegisterPilot(PlayerController newPilot)
        {
            if (IsRegistered)
                throw new Exception($"Error: Drone N°{ID} already have a pilot.");

            Pilot = newPilot;
            newPilot.AssignDrone(this);
        }

        #region Actions
        
        public void Move(Vector2 input)
        {
            Vector3 motion = new Vector3(input.x, 0, input.y) * (_moveSpeed * Time.deltaTime);
            _rigidbody.MovePosition(Transform.position + motion);
            //if (_rigidbody.velocity.magnitude > 0.1f) {
            transform.forward = new Vector3(input.x, 0, input.y);
        }
        
        private void Operate()
        {
            switch (_nearestPOI.Type)
            {
                case POIType.CrystalDeposit:
                    Mine();
                    break;
                default:
                    throw new ArgumentException($"Drone #{ID} try to operate on an unknow [{_nearestPOI.Type}] POI type");
            }
        }
        
        private void Interact()
        {
        }

        #endregion
        
        private void Mine()
        {
            CrystalDeposit deposit = _nearestPOI as CrystalDeposit;

            if (!deposit)
            {
                SetMiningVFX(false);
                return;
            }
            SetMiningVFX(true);
            float minedAmount = _miningSpeed * Time.deltaTime * deposit.MiningSpeedMultiplier;
            _accumulatedMinedAmount += minedAmount;

            int amountToAdd = Mathf.FloorToInt(_accumulatedMinedAmount);
            if (amountToAdd > 0)
            {
                deposit.CurrentCapacity -= amountToAdd;
                GameManager.Instance.Crystals += amountToAdd;
                GameManager.Instance.UpdateCrystals();
                _accumulatedMinedAmount -= amountToAdd;
            }
        }

        public void TakeDamage(int damage)
        {
            ArmorPlates -= damage;

            if (ArmorPlates <= 0)
            {
                Destroy(GameObject);
            }
        }

        private void SetMiningVFX(bool value) {
            ParticleSystem.EmissionModule em = _psMinig.emission;
            em.enabled = value;
        }
    }
}
