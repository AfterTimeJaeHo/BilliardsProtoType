using Aftertime.SecretSome.Common;
using Aftertime.SecretSome.Content;
using Cysharp.Threading.Tasks;
using StateMachine.Runtime;
using Waving.Scene;

namespace Waving.Content
{
    using StateMachine = StateMachine.Runtime.StateMachine;
    
    public class BattleContent : IContent
    {
        public ContentState State { get; }
        
        private BattleStateMachine stateMachine;
        
        public void StartContent()
        {
            SceneEntryManager.Instance.Additive(GameScene.SlotPrototype);
            stateMachine = new BattleStateMachine();
            stateMachine.Init(null);
            UpdateExecutor.onUpdate += stateMachine.Execute;
        }

        public void PauseContent()
        {
            UpdateExecutor.onUpdate -= stateMachine.Execute;
        }

        public UniTask StartContentAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask StopContentAsync()
        {
            throw new System.NotImplementedException();
        }

        public void StopContent()
        {
            UpdateExecutor.onUpdate -= stateMachine.Execute;
        }

        public void ResumeContent()
        {
            UpdateExecutor.onUpdate += stateMachine.Execute;
        }
    }
    
    public enum BattleContentsState
    {
        PlayerTurn,
        EnemyTurn
    }
   
}