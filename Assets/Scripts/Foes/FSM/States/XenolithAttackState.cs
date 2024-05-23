using UnityEngine;

using Convoy;
using Convoy.Drones;

namespace Foes.FSM.States
{
    public class XenolithAttackState: XenolithBaseState
    {
        private Xenolith Xenolith { get; set; }

        private ConvoyManager _convoy;
        private Drone _targetDrone;
        private float _internalTimer;
        
        public override void EnterState(Xenolith xenolith)
        {
            Xenolith = xenolith;
            StopAgent();
        }

        public override void UpdateState(Xenolith xenolith)
        {
            switch (Xenolith.Target.tag)
            {
                case "Convoy":
                    _convoy = Xenolith.Target.GetComponent<ConvoyManager>();
                    AttackConvoy();
                    break;
                case "Drone":
                    TrackingDrone();
                    break;
            }
        }
        
        public override void OnTriggerEnter(Xenolith xenolith, Collider other)
        {
            // Xenolith don't interact with anything in this state;
        }

        public override void OnTriggerExit(Xenolith xenolith, Collider other)
        {
            if (!other.CompareTag("Drone")) return;
            
            // Drone is too far, switching to TargetConvoy state
            xenolith.SwitchState(xenolith.TargetConvoyState); //TODO
        }

        private void AttackConvoy()
        {
            if (_internalTimer >= Xenolith.AttackSpeed)
            {
                _convoy.TakeDamage(Xenolith.AttackDamage);
                _internalTimer = 0f;
                return;
            }

            _internalTimer += Time.deltaTime;
        }
        
        private void TrackingDrone()
        {
            throw new System.NotImplementedException();
        }
        
        private void StopAgent()
        {
            Xenolith.Agent.isStopped = true;
            Xenolith.Agent.enabled = false;
            Xenolith.Obstacle.enabled = true;
            // TerrainManager.Instance.UpdateNavMesh();
        }
    }
}
