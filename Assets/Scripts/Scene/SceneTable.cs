using UnityEngine;

namespace Waving.Scene
{
    [CreateAssetMenu(menuName = "Waving/Scene/SceneTable")]
    public class SceneTable : ScriptableObject
    {
        [System.Serializable]
        public class SceneInfo
        {
            public GameScene gameScene;
            public string sceneName;
        }

        [SerializeField] private SceneInfo[] scenes;

        public SceneInfo GetSceneInfo(GameScene gameScene)
        {
            foreach (SceneInfo info in scenes)
            {
                if (info.gameScene == gameScene) 
                    return info;   
            }

            return null;
        }

        public string GetName(GameScene gameScene)
        {
            foreach (SceneInfo info in scenes)
            {
                if (info.gameScene == gameScene) 
                    return info.sceneName;   
            }

            Debug.LogError($"Scene name not found for {gameScene}");
            return "";
        }
    }   
}