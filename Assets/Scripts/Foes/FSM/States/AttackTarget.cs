using Convoy.Drones;

namespace Foes.FSM.FredStates {
    public class AttackTarget : BaseState {
        
        public AttackTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState() {
            Xenolith.agent.SetDestination(Xenolith.transform.position);
        }
        
        public override void UpdateState() {
            // Rotate toward the target
            Xenolith.transform.LookAt(Xenolith.agent.transform.position);
            if (Xenolith.AttackReady) Xenolith.Target.GetComponent<IDamageable>().TakeDamage(Xenolith.attackDamage);
        }
        
        public override void ExitState() { }
        
    }
}
