using System;
using Aftertime.MyTinyStreamer.Slots;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Waving.Battle;

namespace Waving.Di
{
    public class BattleDIInstaller : MonoBehaviour
    {
        [SerializeField] private Image _BattleEnterBG;
        [SerializeField] private Image _BattleEnterCharacter;
        [SerializeField] private CanvasGroup _BattleEnterCanvasGroup;
        [SerializeField] private CanvasGroup _BattleMainCanvasGroup;
        [SerializeField] private Player _player;
        [SerializeField] private Enemy _enemy;
        [SerializeField] private SlotController _slotController;
        [SerializeField] private RectTransform _stageHolder;
        [SerializeField] private CanvasGroup _stageCanvasGroup;
        [SerializeField] private CanvasGroup _uiCanvasGroup;
        [SerializeField] private Image _hSceneImage;
        [SerializeField] private Image _shotImage;

        private void Awake()
        {
            new PlayerTurnContainer(_player, _enemy, _slotController);
            new BattleContentContainer(_BattleEnterBG, _BattleEnterCharacter, _BattleEnterCanvasGroup,
                    _BattleMainCanvasGroup, _player, _enemy);
            new EnemyTurnContainer(_player,_enemy,_stageHolder);
            new HSceneContainer(_hSceneImage,_shotImage,_stageCanvasGroup,_uiCanvasGroup);
        }
    }
}