using Convoy.Drones;
using UnityEngine;

namespace Foes.FSM.States
{
    public class XenolithTargetDroneState: XenolithBaseState
    {
        private Drone _drone;
        
        public override void EnterState(Xenolith xenolith)
        {
            Debug.Log($"{xenolith.name}: detected {xenolith.Target.name}");
            _drone = xenolith.Target.GetComponent<Drone>();
        }

        public override void UpdateState(Xenolith xenolith)
        {
            if (!_drone)
                xenolith.SwitchState(xenolith.TargetConvoyState);
            
            float distanceToDrone = Vector3.Distance(xenolith.transform.position, _drone.transform.position);
            
            if (distanceToDrone > xenolith.DetectionRadius)
                xenolith.SwitchState(xenolith.TargetConvoyState);
            else
                xenolith.Agent.SetDestination(_drone.transform.position);
        }

        public override void OnTriggerEnter(Xenolith xenolith, Collider other)
        {
            if (!other.CompareTag("Drone")) return;
            
            // TODO: Attack animation > HERE <
            _drone.TakeDamage();
            xenolith.SwitchState(xenolith.TargetConvoyState);
        }

        public override void OnTriggerExit(Xenolith xenolith, Collider other)
        {
            
        }
    }
}
