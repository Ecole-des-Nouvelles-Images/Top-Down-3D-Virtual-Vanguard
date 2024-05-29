using Managers;
using Player;
using UnityEngine;

namespace Convoy.Modules
{
    public class Head : Module
    {
        private void OnValidate()
        {
            if (MaximumControllers != 4)
            {
                MaximumControllers = 4;
                Debug.Log("Warning: Convoy head have a constant player capacity [4];");
            }
        }

        public override bool EnterModule(PlayerController newController)
        {
            base.EnterModule(newController);
            
            if (IsFull && GameManager.Instance.IsInTransit)
            {
                GameManager.Instance.OnStopTransit.Invoke();
                Deactivate();
                Online = true;
            }

            return true;
        }

        //
        
        public override void Operate(PlayerController currentController)
        {
            // Does nothing;
        }
    }
}
