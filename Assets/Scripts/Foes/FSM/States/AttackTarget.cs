using Convoy.Drones;

namespace Foes.FSM.States {
    public class AttackTarget : BaseState {
        
        public AttackTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState() {
            Xenolith.Agent.SetDestination(Xenolith.transform.position);
        }
        
        public override void UpdateState() {
            // Rotate toward the target
            Xenolith.transform.LookAt(Xenolith.Agent.transform.position);
            if (Xenolith.AttackReady) {
                Xenolith.Target.GetComponent<IDamageable>().TakeDamage(Xenolith.AttackDamage);
                Xenolith.Animator.SetTrigger("Attack");
            }

        }
        
        public override void ExitState() { }
        
    }
}
