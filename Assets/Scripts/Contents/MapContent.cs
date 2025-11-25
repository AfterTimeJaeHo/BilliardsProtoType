using Aftertime.SecretSome.Content;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Waving.Battle;
using Waving.Common;
using Waving.Content;
using Waving.Di;
using Waving.Scene;

namespace Waving.Tile.Content
{
    public class MapContent : DIClass,IContent
    {
        public ContentState State { get; }
        
        [Inject] private MapContentContainer _container;
        private GameObject _enemyInBattle;
        
        public async void StartContent()
        {
            await SceneEntryManager.Instance.Additive(GameScene.Map);
            DIContainerBase.TryInjectAll(this);
            _container.DiceMoveController.onEnemyFaced += (_) => ContentRunner.PauseContent<MapContent>();
            _container.DiceMoveController.onEnemyFaced += (enemy) => _enemyInBattle = enemy;
        }

        public void PauseContent()
        {
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
        }

        public void ResumeContent()
        {
            if(_enemyInBattle != null)
                GameObject.Destroy(_enemyInBattle);
        }
    }
   
}