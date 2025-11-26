using UnityEngine;
using UnityEngine.UI;
using Waving.Battle;
using Waving.Common;

namespace Waving.Di
{
    [ForTypes(typeof(AssaultState))]
    public class AssaultContainer : DIContainerBase
    {
        public SpriteGroup SpriteGroup { get; private set; }
        public SpriteRenderer BGSr { get; private set; }
        public SpriteRenderer EnemySr { get; private set; }
        public Transform DestinationTransform { get; private set; }
        public Transform TargetTransform { get; private set; }
        public Camera BattleCam {get; private set;}
        public AssaultSlider AssaultSlider { get; private set; }
        public CanvasGroup AssaultCanvasGroup { get; private set; }

        public AssaultContainer(SpriteGroup spriteGroup, SpriteRenderer bgSr, SpriteRenderer enemySr,
            Transform destinationTransform, Transform targetTransform, Camera battleCam, AssaultSlider assaultSlider,
            CanvasGroup assaultCanvasGroup)
        {
            SpriteGroup = spriteGroup;
            BGSr = bgSr;
            EnemySr = enemySr;
            DestinationTransform = destinationTransform;
            TargetTransform = targetTransform;
            BattleCam = battleCam;
            AssaultSlider = assaultSlider;
            AssaultCanvasGroup = assaultCanvasGroup;
        }
    }
   
}