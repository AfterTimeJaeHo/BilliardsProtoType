using System;

namespace StateMachine.Runtime {
    [Serializable]
    public class TransitionState : State {
        public StateCondition Condition;
    }
}