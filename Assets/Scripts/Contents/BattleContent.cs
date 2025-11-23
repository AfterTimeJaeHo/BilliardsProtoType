using Aftertime.SecretSome.Content;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Waving.Scene;

namespace MyNamespace
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
        }
    }
   
}