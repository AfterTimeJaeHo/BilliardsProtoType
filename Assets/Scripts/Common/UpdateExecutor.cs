using System;
using UnityEngine;

namespace Aftertime.SecretSome.Common
{
    public class UpdateExecutor : MonoBehaviour
    {
        public static Action onUpdate = delegate { };
        
        private void Update()
        {
            onUpdate();
        }
    }
   
}
