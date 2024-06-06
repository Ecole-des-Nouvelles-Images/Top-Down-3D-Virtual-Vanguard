using Game.Player;

namespace Game.Convoy.Modules
{
    public class Head : Module
    {
        public static bool InteractionReady = true;
        
        protected void Start()
        {
            MaximumControllers = 0;
        }

        public override bool EnterModule(PlayerController newController)
        {
            if (!InteractionReady) return false;
            
            base.EnterModule(newController);
            
            if (Controllers.Count == MaximumControllers)
            {
                if (GameManager.Instance.IsInTransit)
                    GameManager.Instance.OnStopTransit.Invoke();
                else if (!GameManager.Instance.IsInTransit)
                    GameManager.Instance.OnStartTransit.Invoke();

                Deactivate();
                Online = true;
                InteractionReady = false;
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
