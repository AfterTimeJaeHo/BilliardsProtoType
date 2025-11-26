using System;
using Aftertime.MyTinyStreamer.Slots;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Waving.Battle;
using Waving.Common;

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
        [SerializeField] private RawImage _playerSkillBG;
        [SerializeField] private RawImage _playerSkillCharacter;

        [SerializeField] private RectTransform _stageHolder;
        [SerializeField] private CanvasGroup _stageCanvasGroup;
        [SerializeField] private CanvasGroup _uiCanvasGroup;
        [SerializeField] private Image _hSceneImage;
        [SerializeField] private Image _shotImage;

        [SerializeField] private SpriteGroup _assaultGroup;
        [SerializeField] private SpriteRenderer _bgSr;
        [SerializeField] private SpriteRenderer _enemySr;
        [SerializeField] private Transform _destinationTransform;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Camera _battleCam;
        [SerializeField] private AssaultSlider _assaultSlider;
        [SerializeField] private CanvasGroup _assaultCanvasGroup;

        private void Awake()
        {
            new AssaultContainer(_assaultGroup, _bgSr, _enemySr, _destinationTransform, _targetTransform, _battleCam,
                _assaultSlider, _assaultCanvasGroup);
            new PlayerTurnContainer(_player, _enemy, _slotController, _playerSkillBG, _playerSkillCharacter);
            new BattleContentContainer(_BattleEnterBG, _BattleEnterCharacter, _BattleEnterCanvasGroup,
                _BattleMainCanvasGroup, _player, _enemy);
            new EnemyTurnContainer(_player, _enemy, _stageHolder);
            new HSceneContainer(_hSceneImage, _shotImage, _stageCanvasGroup, _uiCanvasGroup);
        }
    }
}