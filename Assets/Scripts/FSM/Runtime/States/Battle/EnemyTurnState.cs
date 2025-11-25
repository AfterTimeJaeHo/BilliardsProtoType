using System;
using Aftertime.SecretSome.UI.Popup;
using SRPG;
using Waving.Di;
using Waving.UI;

namespace StateMachine.Runtime {
    public class EnemyTurnState : DIClass,IState 
    {
        public event Action onAttack;
        
        public OnEnter onEnter { get; set; }
        public OnExecute onExecute { get; set; }
        public OnExit onExit { get; set; }
        public async void Enter()
        {
            TurnChangePopup turnChangePopup = PopupManager.Instance.GetPopup<TurnChangePopup>();
            await turnChangePopup.UpdateEnemyTurnView();
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}