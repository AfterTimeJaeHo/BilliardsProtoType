using StateMachine.Runtime;
using UnityEngine;
using Waving.Battle;

namespace Waving.Di
{
    [ForTypes(typeof(EnemyTurnState))]
    public class EnemyTurnContainer : DIContainerBase
    {
        public Player Player { get; private set; }
        public Enemy Enemy { get; private set; }
        public RectTransform StageHolder { get; private set; }
        
        public EnemyTurnContainer(Player player,Enemy enemy,RectTransform stageHolder)
        {
            Player = player;
            Enemy = enemy;
            StageHolder = stageHolder;
        }
    }
   
}