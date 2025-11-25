using Aftertime.MyTinyStreamer.Slots;
using StateMachine.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Waving.Battle;

namespace Waving.Di
{
    [ForTypes(typeof(PlayerTurnState))]
    public class PlayerTurnContainer : DIContainerBase
    {
        public Player Player { get; private set; }
        public Enemy Enemy { get; private set; }
        public SlotController SlotController { get; private set; }
        public RawImage PlayerSkillBG { get; private set; }
        public RawImage PlayerSkillCharacter { get; private set; }

        public PlayerTurnContainer(Player player, Enemy enemy, SlotController slotController, RawImage playerSkillBG,
            RawImage playerSkillCharacter)
        {
            Player = player;
            Enemy = enemy;
            SlotController = slotController;
            PlayerSkillBG = playerSkillBG;
            PlayerSkillCharacter = playerSkillCharacter;
        }
    }
}