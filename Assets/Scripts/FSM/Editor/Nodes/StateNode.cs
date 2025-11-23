using System;
using Unity.GraphToolkit.Editor;

namespace StateMachine.Editor {
    [Serializable]
    internal class StateNode : StateMachineNode {
        const int MAX_TRANSITIONS = 2;
        const string TRANSITION_PORT_PREFIX = "Transition";
        const string DIRECT_STATE_PORT_NAME = "StateLink";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            string optionName = "stateName";
            Type dataType = typeof(string);
            context.AddOption(optionName, dataType);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context) {
            context.AddInputPort(EXECUTION_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(DIRECT_STATE_PORT_NAME)
                .WithDisplayName("State")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            for (int i = 0; i < MAX_TRANSITIONS; i++) {
                context.AddOutputPort($"{TRANSITION_PORT_PREFIX}{i}")
                    .WithDisplayName($"Transition {i + 1}")
                    .WithConnectorUI(PortConnectorUI.Arrowhead)
                    .Build();
            }
        }
    }
}
