using System;
using SRPG;
using Waving.Di;

namespace StateMachine.Runtime {
    public class EnemyTurnState : DIClass,IState 
    {
        public OnEnter onEnter { get; set; }
        public OnExecute onExecute { get; set; }
        public OnExit onExit { get; set; }
        public void Enter()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }
    }
}