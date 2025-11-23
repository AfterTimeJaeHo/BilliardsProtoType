using System.Collections.Generic;
using SRPG;
using Waving.Content;

namespace StateMachine.Runtime
{
    public class BattleStateMachine : StateMachine
    {
        private Dictionary<BattleContentsState, IState> states;

        public override void Init(IState state)
        {
            states = new Dictionary<BattleContentsState, IState>();

            PlayerTurnState playerTurnState = new PlayerTurnState();
            EnemyTurnState monsterTurnState = new EnemyTurnState();
            states.Add(BattleContentsState.PlayerTurn, playerTurnState);
            states.Add(BattleContentsState.EnemyTurn, monsterTurnState);
            
            base.Init(playerTurnState);
        }
    }
   
}