namespace StateMachine.Runtime {
    public interface IStateMachineNodeExecutor<in TNode> where TNode : State {
        bool Execute(TNode node, StateMachineDirector ctx);
    }
}