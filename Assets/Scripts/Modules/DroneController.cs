using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Modules
{
    public class DroneController: Module
    {
        [Header("References")]
        public GameObject DronePrefab;
        public List<Drone> Drones = new();
        public List<Anchor> Anchors = new();

        [Header("Gameplay")]
        public float DroneSpeed;
        public float RespawnDelay;

        public static float GlobalDroneSpeed { get; private set; }
        public override float CurrentBattery { get; set; }

        private void Awake()
        {
            GlobalDroneSpeed = DroneSpeed;
        }

        protected override void Start()
        {
            StartCoroutine(RebuildDroneWatcher());
        }
        
        public void ActivateDrone(PlayerInfo issuer, Vector2 value)
        {
            Drone userDrone = Drones.Find(drone => drone.IsRegistered && drone.User == issuer);

            if (userDrone == null)
            {
                userDrone = Drones.Find(drone => !drone.IsRegistered);
                if (userDrone == null) {
                    Debug.Log("No available drone to control");
                    return;
                }
                
                userDrone.RegisterPlayer(issuer);
                userDrone.UpdateDirection(value);
            }
            
            userDrone.UpdateDirection(value);
        }

        private IEnumerator RebuildDroneWatcher()
        {
            while (gameObject)
            {
                if (Drones.Count < 4)
                {
                    yield return new WaitForSeconds(RespawnDelay);
                    Anchor availableAnchor = FindFirstAvailableAnchor();
                    Drone drone = Instantiate(DronePrefab, availableAnchor.Position, Quaternion.identity, null).GetComponent<Drone>();
                    availableAnchor.Occupant = drone;
                    drone.Anchor = availableAnchor;
                    Drones.Add(drone);
                }

                yield return null;
            }
        }

        public void DockAllDrone()
        {
            foreach (Drone drone in Drones)
                drone.ReturnToAnchor();
        }

        #region Utils

        private Anchor FindFirstAvailableAnchor()
        {
            Anchor availableAnchor = Anchors.Find(anchor => !anchor.IsTaken);

            if (!availableAnchor) throw new Exception("No suitable drone's anchor found when at least one should be");
            return availableAnchor;
        }


        #endregion
    }
}
