using UnityEngine;

namespace Waving.Scene
{
    public class MapSceneEntry : ISceneEntry
    {
        private Camera _camera;

        public void OnEnter()
        {
            if (_camera == null)
                _camera = GameObject.Find("MapCamera").GetComponent<Camera>();
            
            _camera.enabled = true;
        }

        public void OnExit()
        {
            _camera.enabled = false;
        }
    }
}