using System.Collections.Generic;
using Convoy;
using Convoy.Drones;
using UnityEngine;

namespace Foes.FSM.FredStates
{
    public class SelectTarget : BaseState
    {
        public SelectTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState() {
            Xenolith.agent.SetDestination(Xenolith.transform.position);
        }
        
        public override void UpdateState() {
            // Update the target
            Xenolith.Target = Xenolith.FetchNearestDamageable().GameObject;
        }
        
        public override void ExitState() { }

    }
}
