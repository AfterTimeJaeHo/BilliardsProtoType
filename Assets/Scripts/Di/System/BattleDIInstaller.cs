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

        private void Awake()
        {
            new PlayerTurnContainer(_player, _enemy, _slotController);
            new BattleContentContainer(_BattleEnterBG, _BattleEnterCharacter, _BattleEnterCanvasGroup,
                    _BattleMainCanvasGroup, _player, _enemy);
        }
    }
}