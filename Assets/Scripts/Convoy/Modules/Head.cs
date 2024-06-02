using Managers;
using Player;
using UnityEngine;

namespace Convoy.Modules
{
    public class Head : Module
    {
        protected void Start()
        {
            MaximumControllers = 0;
        }

        public override bool EnterModule(PlayerController newController)
        {
            base.EnterModule(newController);
            
            if (PlayerManager.Instance.PlayerNumber == MaximumControllers)
            {
                if (GameManager.Instance.IsInTransit)
                    GameManager.Instance.OnStopTransit.Invoke();
                else if (!GameManager.Instance.IsInTransit)
                    GameManager.Instance.OnStartTransit.Invoke();

                Deactivate();
                Online = true;
            }

            return true;
        }
        
        public override void Operate(PlayerController currentController) {} // Does nothing;

        public void UpdateMaximumControllers(int currentControllersPlaying)
        {
            MaximumControllers = currentControllersPlaying;
        }
    }
}
