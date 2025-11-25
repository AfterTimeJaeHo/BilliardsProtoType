using System.Collections.Generic;
using SRPG;
using Waving.Battle;
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
            HSceneState hSceneState = new HSceneState();
            states.Add(BattleContentsState.PlayerTurn, playerTurnState);
            states.Add(BattleContentsState.EnemyTurn, monsterTurnState);
            states.Add(BattleContentsState.HScene, hSceneState);

            playerTurnState.onSlotEvaluated += () => ChangeState(monsterTurnState);
            playerTurnState.onEnemyDead += () => ChangeState(hSceneState);
            monsterTurnState.onAttack += () => ChangeState(playerTurnState);
            
            base.Init(playerTurnState);
        }
    }
   
}