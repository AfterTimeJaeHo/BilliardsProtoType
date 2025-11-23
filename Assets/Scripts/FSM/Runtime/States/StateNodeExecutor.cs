using UnityEngine;

namespace StateMachine.Runtime {
    public class StateNodeExecutor : IStateMachineNodeExecutor<State> {
        public bool Execute(State node, StateMachineDirector ctx) {
            Debug.Log($"Entering state: {node.StateName}");
            return true;
        }
    }
}