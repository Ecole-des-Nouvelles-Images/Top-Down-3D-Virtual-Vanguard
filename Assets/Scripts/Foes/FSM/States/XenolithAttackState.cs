using UnityEngine;

namespace Foes.FSM.States
{
    public class XenolithAttackState: XenolithBaseState
    {
        private Xenolith Xenolith { get; set; }

        private float _internalTimer;
        
        public override void EnterState(Xenolith xenolith)
        {
            Xenolith = xenolith;
            StopAgent();
        }

        public override void UpdateState(Xenolith xenolith)
        {
            if (_internalTimer >= Xenolith.AttackSpeed)
            {
                Xenolith.Target.TakeDamage(Xenolith.AttackDamage);
                _internalTimer = 0f;
                return;
            }

            _internalTimer += Time.deltaTime;
        }

        public override void OnTriggerEnter(Xenolith xenolith, Collider other)
        {
            // Xenolith don't interact with anything in this state;
        }
        
        public void StopAgent()
        {
            Xenolith.Agent.isStopped = true;
            Xenolith.Agent.enabled = false;
            Xenolith.Obstacle.enabled = true;
            // TerrainManager.Instance.UpdateNavMesh();
        }
    }
}
