using UnityEngine;

namespace Waving.Scene
{
    public class BattleSceneEntry : ISceneEntry
    {
        private Camera _camera;
        public void OnEnter()
        {
            if (_camera == null)
                _camera = GameObject.Find("BattleCamera").GetComponent<Camera>();
            
            _camera.enabled = true;
        }

        public void OnExit()
        {
            _camera.enabled = false;
        }
    }
   
}