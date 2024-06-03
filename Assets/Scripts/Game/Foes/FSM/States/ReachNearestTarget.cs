using UnityEngine;

namespace Game.Foes.FSM.States
{
    public class ReachNearestTarget : BaseState
    {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        
        public ReachNearestTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState() {
            Xenolith.Animator.SetBool(IsMoving, true);
        }

        public override void UpdateState() {
            GameObject target = Xenolith.FetchNearestDamageable().GameObject;
            if (Xenolith.Target != target) Xenolith.Target = target;
            if (!Xenolith.Target) return;
            Xenolith.Agent.SetDestination(Xenolith.Target.transform.position);
        }

        public override void ExitState() {
            Xenolith.Agent.SetDestination(Xenolith.transform.position);
            Xenolith.Animator.SetBool(IsMoving, false);
        }
    }
}
