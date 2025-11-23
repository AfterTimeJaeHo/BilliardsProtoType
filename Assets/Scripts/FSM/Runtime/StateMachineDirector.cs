using System.Collections.Generic;
using UnityEngine;

namespace StateMachine.Runtime {
    public class StateMachineDirector : MonoBehaviour {
        [Header("Graph")]
        public StateMachineRuntimeGraph RuntimeGraph;
        
        Dictionary<System.Type, object> executors;
        private State currentNode;

        void Awake() {
            executors = new Dictionary<System.Type, object> {
                { typeof(StartState), new StartNodeExecutor() },
                { typeof(State), new StateNodeExecutor() },
                { typeof(TransitionState), new TransitionNodeExecutor() }
            };
        }

        void Start() {
            if (RuntimeGraph == null) {
                Debug.LogError("No runtime graph assigned!");
                return;
            }
            
            currentNode = RuntimeGraph.Nodes[0];
        }

        void Update() {
            if (currentNode == null) return;

            if (!executors.TryGetValue(currentNode.GetType(), out var executor)) {
                Debug.LogError($"No executor found for node type: {currentNode.GetType()}");
                currentNode = null;
                return;
            }

            if (currentNode is State stateNode) {
                var stateExecutor = (IStateMachineNodeExecutor<State>)executor;
                stateExecutor.Execute(stateNode, this);

                State directStateTarget = null;

                foreach (var nextIndex in stateNode.NextNodeIndices) {
                    var nextNode = RuntimeGraph.Nodes[nextIndex];

                    if (nextNode is TransitionState transitionNode) {
                        var transitionExecutor = (IStateMachineNodeExecutor<TransitionState>)executors[typeof(TransitionState)];

                        if (transitionExecutor.Execute(transitionNode, this) && transitionNode.NextNodeIndices.Count > 0) {
                            currentNode = RuntimeGraph.Nodes[transitionNode.NextNodeIndices[0]];
                            return;
                        }
                    }
                    else if (nextNode is State && directStateTarget == null) {
                        directStateTarget = nextNode;
                    }
                }

                if (directStateTarget != null) {
                    currentNode = directStateTarget;
                    return;
                }
            }
            else if (currentNode is TransitionState transitionNode) {
                var transitionExecutor = (IStateMachineNodeExecutor<TransitionState>)executor;

                if (transitionExecutor.Execute(transitionNode, this) && transitionNode.NextNodeIndices.Count > 0) {
                    currentNode = RuntimeGraph.Nodes[transitionNode.NextNodeIndices[0]];
                }
                else {
                    currentNode = null;
                }
            }
            else if (currentNode is StartState startNode) {
                var startExecutor = (IStateMachineNodeExecutor<StartState>)executor;
                startExecutor.Execute(startNode, this);
                currentNode = startNode.NextNodeIndices.Count > 0
                    ? RuntimeGraph.Nodes[startNode.NextNodeIndices[0]]
                    : null;
            }
        }
    }
}