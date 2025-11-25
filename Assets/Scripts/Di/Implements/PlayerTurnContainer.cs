using Aftertime.MyTinyStreamer.Slots;
using StateMachine.Runtime;
using TMPro;
using UnityEngine;
using Waving.Battle;

namespace Waving.Di
{
    [ForTypes(typeof(PlayerTurnState))]
    public class PlayerTurnContainer : DIContainerBase
    {
        public Player Player { get; private set; }
        public Enemy Enemy { get; private set; }
        public SlotController SlotController { get; private set; }

        public PlayerTurnContainer(Player player,Enemy enemy,SlotController slotController)
        {
            Player = player;
            Enemy = enemy;
            SlotController = slotController;
        }
    }
}