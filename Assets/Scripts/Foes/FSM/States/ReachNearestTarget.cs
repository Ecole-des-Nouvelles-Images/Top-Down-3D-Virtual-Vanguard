using UnityEngine;

namespace Foes.FSM.FredStates
{
    public class ReachNearestTarget : BaseState
    {
        public ReachNearestTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState() { }

        public override void UpdateState() {
            GameObject target = Xenolith.FetchNearestDamageable().GameObject;
            if (Xenolith.Target != target) Xenolith.Target = target;
            if (!Xenolith.Target) return;
            Xenolith.agent.SetDestination(Xenolith.Target.transform.position);
        }

        public override void ExitState() {
            Xenolith.agent.SetDestination(Xenolith.transform.position);
        }
    }
}
