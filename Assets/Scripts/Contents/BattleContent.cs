using Aftertime.SecretSome.Content;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Waving.Common;
using Waving.Content;
using Waving.Scene;

namespace Waving.Content
{
    public class BattleContent : IContent
    {
        public ContentState State { get; }
        
        public void StartContent()
        {
            SceneEntryManager.Instance.Additive(GameScene.SlotPrototype);
        }

        public void PauseContent()
        {
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
        }

        public void ResumeContent()
        {
            throw new System.NotImplementedException();
        }
    }
   
}