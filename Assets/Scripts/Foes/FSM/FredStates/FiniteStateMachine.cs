using System;

namespace Foes.FSM.FredStates
{
    public class FiniteStateMachine {
        private BaseState _currentState;
        private Xenolith _xenolith;

        public FiniteStateMachine(Xenolith xenolith) {
            this._xenolith = xenolith;
        }

        public void Update() {
            if (_currentState == null) return;
            
            // Execute current state
            _currentState.UpdateState();
            
            // Check transition from current state
            switch (_currentState) {
                case SelectTarget:
                    if (_xenolith.Target) ChangeState(new ReachTarget(_xenolith));
                    break;
                case AttackTarget:
                    if (!_xenolith.Target) ChangeState(new SelectTarget(_xenolith));
                    if (_xenolith.Target && !_xenolith.TargetIsReachable) ChangeState(new ReachTarget(_xenolith));
                    break;
                case ReachTarget:
                    if (_xenolith.Target && _xenolith.TargetIsReachable) ChangeState(new AttackTarget(_xenolith));
                    else ChangeState(new ReachTarget(_xenolith));
                    break;
                default:
                    throw new Exception($"{_currentState.GetType().Name} is not a valid state");
            }
        }

        public void ChangeState(BaseState newState) {
            if (_currentState != null) {
                _currentState.ExitState();
            }
            _currentState = newState;
            if (_currentState != null) {
                _currentState.EnterState();
            }
        }
    }
}
