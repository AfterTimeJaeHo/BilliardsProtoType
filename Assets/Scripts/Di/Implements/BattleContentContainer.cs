using UnityEngine;
using UnityEngine.UI;
using Waving.Battle;
using Waving.Content;

namespace Waving.Di
{
    [ForTypes(typeof(BattleContent))]
    public class BattleContentContainer : DIContainerBase
    {
        public Image EnterBGImage { get; private set; }
        public Image EnterCharacterImage { get; private set; }
        public CanvasGroup EnterCanvasGroup { get; private set; }
        public CanvasGroup MainCanvasGroup { get; private set; }
        public Player Player { get; private set; }
        public Enemy Enemy { get; private set; }

        public BattleContentContainer(Image enterBGImage, Image enterCharacterImage, CanvasGroup enterCanvasGroup,
            CanvasGroup battleMainCanvasGroup,Player player,Enemy enemy)
        {
            EnterBGImage = enterBGImage;
            EnterCharacterImage = enterCharacterImage;
            EnterCanvasGroup = enterCanvasGroup;
            MainCanvasGroup = battleMainCanvasGroup;
            Player = player;
            Enemy = enemy;
        }
    }
}