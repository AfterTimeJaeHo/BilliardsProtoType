using Aftertime.MyTinyStreamer.Tile;
using UnityEngine;

namespace Waving.Di
{
    public class MapDIInstaller : MonoBehaviour
    {
        [SerializeField] private DiceMoveController _diceMoveController;

        private void Awake()
        {
            new MapContentContainer(_diceMoveController);
        }
    }
}