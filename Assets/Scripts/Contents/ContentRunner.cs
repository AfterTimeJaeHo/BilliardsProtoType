using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Waving.Scene;

namespace Aftertime.SecretSome.Content
{
    public class ContentRunner : MonoBehaviour
    {
        public static ContentType CurContentType { get; private set; }
        public event Action onContentInit = delegate { };
        private static Dictionary<Type, IContent> _contents;
        
        /// <summary>지정된 콘텐츠 타입을 시작합니다.</summary>
        public static void StartContent<T>()
        {
            _contents[typeof(T)].StartContent();
        }

        public static void PauseContent<T>()
        {
            _contents[typeof(T)].PauseContent();
        }

        public static void ResumeContent<T>()
        {

            _contents[typeof(T)].ResumeContent();
        }

        public static void StopContent<T>()
        {
            _contents[typeof(T)].StopContent();
        }

        public static async UniTask StopContentAsync<T>()
        {
            await _contents[typeof(T)].StopContentAsync();
        }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _contents = CreateContents();
            SceneEntryManager.Instance.Additive(GameScene.Map);
        }

        private Dictionary<Type, IContent> CreateContents()
        {
            return ReflectionUtil.CreateAllInstances<IContent>();
        }

        /// <summary>등록된 콘텐츠 인스턴스를 반환합니다.</summary>
        public T GetContent<T>() where T : class, IContent
        {
            if (_contents == null)
                return null;

            IContent content;
            bool hasContent = _contents.TryGetValue(typeof(T), out content);
            if (hasContent == false)
                return null;

            return content as T;
        }

        
        public static void StopAllContents()
        {
            foreach (IContent content in _contents.Values)
            {
                content.StopContent();
            }
        }

        protected virtual void Start()
        {
            Init();
            onContentInit.Invoke();
        }

        private void Init()
        {

        }
    }

    public enum ContentType
    {
        Map,
        Battle
    }
}