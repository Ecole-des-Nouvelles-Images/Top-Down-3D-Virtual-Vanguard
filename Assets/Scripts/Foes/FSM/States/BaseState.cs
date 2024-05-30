﻿namespace Foes.FSM.FredStates
{
    public abstract class BaseState
    {
        protected Xenolith Xenolith;

        protected BaseState(Xenolith xenolith)
        {
            Xenolith = xenolith;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
    }
}
