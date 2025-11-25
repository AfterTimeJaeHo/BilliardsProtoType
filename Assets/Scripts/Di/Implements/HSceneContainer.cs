using UnityEngine;
using UnityEngine.UI;
using Waving.Battle;

namespace Waving.Di
{
    [ForTypes(typeof(HSceneState))]
    public class HSceneContainer : DIContainerBase
    {
        public Image HSceneImage { get; private set; }
        public Image ShotImage { get; private set; }
        public CanvasGroup StageCanvasGroup { get; private set; }
        public CanvasGroup UICanvasGroup { get; private set; }
        
        public HSceneContainer(Image hSceneImage,Image shotImage,CanvasGroup stageCanvasGroup,CanvasGroup uiCanvasGroup)
        {
            HSceneImage = hSceneImage;
            ShotImage = shotImage;
            StageCanvasGroup = stageCanvasGroup;
            UICanvasGroup = uiCanvasGroup;
        }
    }
   
}