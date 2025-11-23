using UnityEngine;

namespace StateMachine.Runtime {
    public class StartNodeExecutor : IStateMachineNodeExecutor<StartState> {
        public bool Execute(StartState node, StateMachineDirector ctx) {
            Debug.Log("Starting State Machine");
            return true;
        }
    }
}