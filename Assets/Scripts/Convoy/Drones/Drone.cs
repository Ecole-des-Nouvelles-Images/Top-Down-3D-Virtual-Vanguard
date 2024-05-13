using System;
using UnityEditor;
using UnityEngine;

using Convoy.Modules;
using Player;

namespace Convoy.Drones
{
    public class Drone: MonoBehaviour
    {
        public static int TotalDroneBuilt;

        public int ID { get; private set; }
        
        public DroneController Controller { get; set; }
        public Anchor AssignedAnchor { get; set; }
        public bool Active { get; set; }
        
        public PlayerController Pilot { get; set; }
        public bool IsRegistered => Pilot != null;

        private CharacterController _characterController;

        #region Debug

        private void OnDrawGizmos()
        {
            string playerID = "(Player > " + (Pilot ? Pilot.PlayerID : "none") + ")";
            string activeStatus = (Active ? "Active " + playerID : "Inactive");
            
            Handles.Label(transform.position + Vector3.up * 0.4f + Vector3.left * 0.2f, $"#{ID}");
            Handles.Label(transform.position + Vector3.up * 0.2f + Vector3.left * 0.2f, activeStatus);
        }

        #endregion

        private void Awake()
        {
            ID = ++TotalDroneBuilt;
            _characterController = GetComponent<CharacterController>();
        }

        private void OnDisable()
        {
            Pilot?.UnassignDrone();
            Pilot = null;
            Controller.UnregisterDrone(this);
        }

        public void Move(Vector2 input)
        {
            Vector3 motion = new Vector3(input.x, 0, input.y) * (DroneController.DroneMoveSpeed * Time.deltaTime);
            _characterController.Move(motion);
        }

        public void RegisterPilot(PlayerController newPilot)
        {
            if (IsRegistered)
                throw new Exception($"Error: Drone N°{ID} already have a pilot.");

            Pilot = newPilot;
            newPilot.AssignDrone(this);
        }
    }
}
