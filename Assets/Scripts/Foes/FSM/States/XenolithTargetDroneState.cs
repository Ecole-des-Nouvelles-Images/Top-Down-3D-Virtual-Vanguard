using System;
using UnityEngine;

namespace Foes.FSM.States
{
    public class XenolithTargetDroneState: XenolithBaseState
    {
        public override void EnterState(Xenolith xenolith)
        {
            Debug.Log($"{xenolith.name} detected a drone !");
        }

        public override void UpdateState(Xenolith xenolith)
        {
            throw new NotImplementedException($"{GetType()} is unimplemented");
        }

        public override void OnTriggerEnter(Xenolith xenolith, Collider other)
        {
            throw new NotImplementedException($"{GetType()} is unimplemented");
        }
    }
}
