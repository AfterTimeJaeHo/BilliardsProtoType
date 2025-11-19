using UnityEngine;

namespace Aftertime.StorylineEngine
{
    public class CachedAsset : MonoBehaviour
    {
        public bool IsInitialized { get; protected set; } = false;

        private Transform cachedTransform;
        public Transform CachedTransform
        {
            get
            {
                if (object.ReferenceEquals(cachedTransform, null))
                {
                    cachedTransform = transform;
                }
                return cachedTransform;
            }
        }

        private GameObject cachedGameObject;
        public GameObject CachedGameObject
        {
            get
            {
                if (object.ReferenceEquals(cachedGameObject, null))
                {
                    cachedGameObject = gameObject;
                }
                return cachedGameObject;
            }
        }

        protected virtual void OnDestroy()
        {
            cachedTransform = null;
            cachedGameObject = null;
        }
    }
}
