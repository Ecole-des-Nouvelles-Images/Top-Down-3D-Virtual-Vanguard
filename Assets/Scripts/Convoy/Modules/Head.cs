using Player;
using UnityEngine;

namespace Convoy.Modules
{
    public class Head : Module
    {
        public override void Operate(PlayerController currentController)
        {
            Debug.LogWarning("This module does not nothing yet");
        }

        public override bool EnterModule(PlayerController newController)
        {
            Debug.LogWarning("This module does not nothing yet");
            return false;
        }
    }
}
