using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Runtime {
    public class StateMachineRuntimeGraph : ScriptableObject {
        [SerializeReference]
        public List<State> Nodes = new();
    }
}