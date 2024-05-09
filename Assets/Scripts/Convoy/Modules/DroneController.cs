using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Convoy.Drones;
using UnityEngine;

using Player;

namespace Convoy.Modules
{
    public class DroneController : Module
    {
        [Header("Drone specifics")] 
        [SerializeField] private GameObject _dronePrefab;
        [SerializeField] private Transform _droneParent;
        [SerializeField] private List<Anchor> _anchors;
        [Space]
        [SerializeField] private float _droneMoveSpeed;
        [SerializeField] private float _droneRebuildTime;
        [SerializeField] private bool _enableDroneAutoRebuild;

        public static float DroneMoveSpeed;
        
        private List<Drone> _drones;

        protected override void Awake()
        {
            base.Awake();
            _drones = new List<Drone>(MaximumControllers);
            DroneMoveSpeed = _droneMoveSpeed;
        }

        private void Start()
        {
            if (_enableDroneAutoRebuild)
                StartCoroutine(AutoRebuildDroneWatcher());
        }

        protected override void Update()
        {
            if (!Online)
            {
                foreach (Drone drone in _drones)
                {
                    drone.Active = false;
                }
                
                Deactivate();
            }
        }

        #region Actions

        public override bool EnterModule(PlayerController pilot)
        {
            if (!base.EnterModule(pilot)) {
                return false;
            }
            
            ActivateDrone(pilot);
            return true;
        }

        public override bool ExitModule(PlayerController pilot)
        {
            DeactivateDrone(pilot);
            return base.ExitModule(pilot);
        }
        
        public override void Operate()
        {
            throw new System.NotImplementedException();
        }

        public override void Interact()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Drone Management

        private void ActivateDrone(PlayerController pilot)
        {
            Drone userDrone = _drones.Find(drone => drone.IsRegistered && drone.Pilot == pilot);

            if (!userDrone) 
                userDrone = RegisterNewDrone(pilot);

            if (!userDrone)
                return;

            userDrone.Active = true;
        }

        private void DeactivateDrone(PlayerController pilot)
        {
            Drone userDrone = _drones.Find(drone => drone.IsRegistered && drone.Pilot == pilot);
            userDrone.Active = false;
        }

        private Drone RegisterNewDrone(PlayerController pilot)
        {
            Drone userDrone = _drones.Find(drone => !drone.IsRegistered);
            
            if (!userDrone) {
                Debug.Log("No available drone to control");
                return null;
            }
                
            userDrone.RegisterPilot(pilot);
            return userDrone;
        }

        public void UnregisterDrone(Drone drone)
        {
            if (! _drones.Remove(drone))
                throw new Exception($"Error: trying to remove a rogue drone. ID #{drone.ID} wasn't registered");
        }

        private IEnumerator AutoRebuildDroneWatcher()
        {
            while (Online)
            {
                if (_drones.Count < 4)
                {
                    yield return new WaitForSeconds(_droneRebuildTime);
                    
                    Anchor availableAnchor = _anchors.First(anchor => anchor.Available);
                    
                    if (!availableAnchor)
                        throw new Exception("No suitable drone anchor found when at least one should be.");

                    Drone drone = Instantiate(_dronePrefab, availableAnchor.Position, Quaternion.identity, _droneParent).GetComponent<Drone>();
                    availableAnchor.Occupant = drone;
                    drone.name = "Drone_" + drone.ID;
                    drone.Controller = this;
                    drone.AssignedAnchor = availableAnchor;
                    _drones.Add(drone);
                }

                yield return null;
            }
        }

        #endregion
    }
}
