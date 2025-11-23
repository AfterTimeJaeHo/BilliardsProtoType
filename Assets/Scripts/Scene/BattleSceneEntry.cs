using UnityEngine;

namespace Waving.Scene
{
    public class BattleSceneEntry : ISceneEntry
    {
        private Camera _battleCam;
        private Camera _mapCam;

        public void OnEnter()
        {
            if (_mapCam == null)
                _mapCam = GameObject.Find("MapCamera").GetComponent<Camera>();

            if (_battleCam == null)
                _battleCam = GameObject.Find("BattleCamera").GetComponent<Camera>();

            _mapCam.gameObject.SetActive(false);
            _battleCam.gameObject.SetActive(true);
        }

        public void OnExit()
        {
            _mapCam.gameObject.SetActive(true);
            _battleCam.gameObject.SetActive(false);
        }
    }
}