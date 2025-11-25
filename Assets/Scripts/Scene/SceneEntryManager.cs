using System.Collections.Generic;
using Aftertime.StorylineEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Waving.MyTinyStreamer.Common;

namespace Waving.Scene
{
    using Scene = UnityEngine.SceneManagement.Scene;
    using SceneInfo = SceneTable.SceneInfo;

    public enum GameScene
    {
        Map,
        Loading,
        SlotPrototype,
    }

    public class SceneEntryManager : SingletonMonoBehaviour<SceneEntryManager>
    {
        [SerializeField] private SceneTable _sceneTable;

        private Dictionary<SceneInfo, ISceneEntry> _entries = new();

        public GameScene CurrentSceneType { get; private set; }

        public ISceneEntry CurrentSceneEntry
        {
            get
            {
                SceneInfo sceneInfo = _sceneTable.GetSceneInfo(CurrentSceneType);
                return _entries[sceneInfo];
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            _sceneTable = GetSceneTable();
            _entries = CreateSceneEntries();
        }

        private SceneTable GetSceneTable()
        {
            string path = Define.SceneTablePath;
            SceneTable sceneTable = Resources.Load<SceneTable>(path);
            return sceneTable;
        }

        private Dictionary<SceneInfo, ISceneEntry> CreateSceneEntries()
        {
            MapSceneEntry MapEntry = new MapSceneEntry();
            BattleSceneEntry battleEntry = new BattleSceneEntry();

            Dictionary<SceneInfo, ISceneEntry> entries = new Dictionary<SceneInfo, ISceneEntry>();
            SceneInfo mapInfo = _sceneTable.GetSceneInfo(GameScene.Map);
            SceneInfo slotInfo = _sceneTable.GetSceneInfo(GameScene.SlotPrototype);
            entries.Add(mapInfo, MapEntry);
            entries.Add(slotInfo, battleEntry);
            return entries;
        }


        public async UniTask ChangeScene(GameScene sceneType)
        {
            // await DirectingManager.FadeOut();

            SceneInfo currentSceneInfo = _sceneTable.GetSceneInfo(CurrentSceneType);
            SceneInfo loadSceneInfo = _sceneTable.GetSceneInfo(sceneType);
            await UnloadScene(currentSceneInfo);
            ExitCurrentSceneEntry();
            
            await LoadSceneAdditive(loadSceneInfo);
            EnterSceneEntry(loadSceneInfo);

            // await DirectingManager.FadeIn();
        }

        public async UniTask Additive(GameScene sceneType)
        {
            SceneInfo sceneInfo = _sceneTable.GetSceneInfo(sceneType);
            await LoadSceneAdditive(sceneInfo);
            EnterSceneEntry(sceneInfo);
        }

        public async UniTask Remove(GameScene sceneType)
        {
            SceneInfo sceneInfo = _sceneTable.GetSceneInfo(sceneType);
            ExitCurrentSceneEntry();
            await UnloadScene(sceneInfo);
        }

        private async UniTask LoadSceneAdditive(SceneInfo sceneInfo)
        {
            string sceneName = sceneInfo.sceneName;
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            CurrentSceneType = sceneInfo.gameScene;
        }

        private async UniTask UnloadScene(SceneInfo sceneInfo)
        {
            string sceneName = sceneInfo.sceneName;
            Scene scene = SceneManager.GetSceneByName(sceneName);
            await SceneManager.UnloadSceneAsync(scene);
        }

        private void EnterSceneEntry(SceneInfo sceneInfo)
        {
            _entries[sceneInfo].OnEnter();
        }

        private void ExitCurrentSceneEntry()
        {
            CurrentSceneEntry.OnExit();
        }
    }
}