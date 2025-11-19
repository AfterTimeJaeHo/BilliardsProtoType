using UnityEngine;

namespace Aftertime.StorylineEngine
{
    public abstract class SingletonMonoBehaviour<T> : CachedAsset where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;

                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).ToString());
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        public static T Seek
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType(typeof(T)) as T;
                return _instance;
            }
        }

        public static bool IsInit
        {
            get
            {
                return _instance != null;
            }
        }

        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                gameObject.name = GetType().Name;
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public abstract void Initialize();

        public virtual void OnApplicationQuit()
        {
            _instance = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
