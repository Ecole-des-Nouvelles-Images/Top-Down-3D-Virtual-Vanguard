using System.Collections.Generic;
using Convoy;
using Convoy.Drones;
using UnityEngine;

namespace Foes.FSM.FredStates
{
    public class SelectTarget : BaseState
    {
        public SelectTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState()
        {
            Debug.Log("Entering SelectTarget state.");
            // Code pour initialiser l'état de sélection de cible
        }

        
        public override void UpdateState()
        {
            List<GameObject> targetableGameObjects = new List<GameObject>();
            
            // Fetch all current modules as target
            ConvoyManager convoyManager = Object.FindObjectOfType<ConvoyManager>();
            foreach (Transform convoyChild in convoyManager.transform) {
                targetableGameObjects.Add(convoyChild.gameObject);
            }
            
            // Fetch all current drones as target
            foreach (Drone drone in Object.FindObjectsOfType<Drone>()) {
                if (!drone.IsTargetable) continue;
                targetableGameObjects.Add(drone.gameObject);
            }
            
            // Select the neareast target
            GameObject nearestTarget = null;
            float nearestDistance = Mathf.Infinity;
            foreach (GameObject target in targetableGameObjects) {
                float distance = Vector3.Distance(Xenolith.transform.position, target.transform.position);
                if (distance >= nearestDistance) continue;
                nearestDistance = distance;
                nearestTarget = target;
            }            
            
            // Update the target
            Xenolith.Target = nearestTarget;
        }
        
        public override void ExitState()
        {
            Debug.Log("Exiting SelectTarget state.");
            // Code pour nettoyer lorsque l'état est quitté
        }

    }
}
