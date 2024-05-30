namespace Foes.FSM.States
{
    public class SelectTarget : BaseState
    {
        public SelectTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState() {
            Xenolith.navMeshAgent.SetDestination(Xenolith.transform.position);
        }
        
        public override void UpdateState() {
            // Update the target
            Xenolith.Target = Xenolith.FetchNearestDamageable().GameObject;
        }
        
        public override void ExitState() { }

    }
}
