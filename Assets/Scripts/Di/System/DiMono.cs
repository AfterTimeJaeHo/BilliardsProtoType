using UnityEngine;

namespace Waving.Di
{
    public class DIMono : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DIContainerBase.TryInjectAll(this);
        }
    }   
}