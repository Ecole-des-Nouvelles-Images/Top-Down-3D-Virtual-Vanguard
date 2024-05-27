using Convoy.Drones;
using UnityEngine;

namespace Foes.FSM.FredStates
{
    public class AttackTarget : BaseState
    {
        public AttackTarget(Xenolith xenolith) : base(xenolith) { }

        public override void EnterState()
        {
            Debug.Log("Entering AttackTarget state.");
            // Code pour initialiser l'état d'attaque de la cible
        }

        public override void ExitState()
        {
            Debug.Log("Exiting AttackTarget state.");
            // Code pour nettoyer lorsque l'état est quitté
        }

        public override void UpdateState()
        {
            if (Xenolith.AttackReady) Xenolith.Target.GetComponent<IDamageable>().TakeDamage(Xenolith.AttackDamage);
            
            Debug.Log("Updating AttackTarget state.");
            // Code pour mettre à jour la logique d'attaque de la cible
        }
    }
}
