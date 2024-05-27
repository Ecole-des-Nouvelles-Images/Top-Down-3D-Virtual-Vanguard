using UnityEngine;

namespace Foes.FSM.FredStates
{
    public class ReachTarget : BaseState
    {
        public ReachTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState()
        {
            Debug.Log("Entering ReachTarget state.");
            // Code pour initialiser l'état d'atteinte de la cible
        }

        public override void UpdateState()
        {
            if (!Xenolith.Target) return;
            Xenolith.Agent.SetDestination(Xenolith.Target.transform.position);
        }
        
        public override void ExitState()
        {
            Debug.Log("Exiting ReachTarget state.");
            // Code pour nettoyer lorsque l'état est quitté
        }
    }
}
