using UnityEngine;

namespace Foes.FSM.States
{
    public class ReachNearestTarget : BaseState
    {
        public ReachNearestTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState() {
            Xenolith.animator.SetBool("IsMoving", true);
        }

        public override void UpdateState() {
            GameObject target = Xenolith.FetchNearestDamageable().GameObject;
            if (Xenolith.Target != target) Xenolith.Target = target;
            if (!Xenolith.Target) return;
            Xenolith.navMeshAgent.SetDestination(Xenolith.Target.transform.position);
        }

        public override void ExitState() {
            Xenolith.navMeshAgent.SetDestination(Xenolith.transform.position);
            Xenolith.animator.SetBool("IsMoving", false);
        }
    }
}
