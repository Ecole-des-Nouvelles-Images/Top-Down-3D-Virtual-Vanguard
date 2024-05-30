using Managers;
using Player;

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
            
            if (PlayerManager.Instance.PlayerNumber == Controllers.Count)
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
