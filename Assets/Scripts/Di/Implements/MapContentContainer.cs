using Aftertime.MyTinyStreamer.Tile;
using Waving.Tile.Content;

namespace Waving.Di
{
    [ForTypes(typeof(MapContent))]
    public class MapContentContainer : DIContainerBase
    {
        public DiceMoveController DiceMoveController { get; private set; }
        
        public MapContentContainer(DiceMoveController diceMoveController)
        {
            DiceMoveController = diceMoveController;
        }
    }
   
}