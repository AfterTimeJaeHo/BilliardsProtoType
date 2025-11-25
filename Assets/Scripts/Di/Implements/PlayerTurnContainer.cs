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
        public Image PlayerSkillBG { get; private set; }
        public Image PlayerSkillCharacter { get; private set; }

        public PlayerTurnContainer(Player player, Enemy enemy, SlotController slotController, Image playerSkillBG,
            Image playerSkillCharacter)
        {
            Player = player;
            Enemy = enemy;
            SlotController = slotController;
            PlayerSkillBG = playerSkillBG;
            PlayerSkillCharacter = playerSkillCharacter;
        }
    }
}