using Aftertime.MyTinyStreamer.Slots;
using Cysharp.Threading.Tasks;
using SRPG;
using Waving.Battle;
using Waving.Di;

namespace StateMachine.Runtime
{
    public class PlayerTurnState : DIClass, IState
    {
        public OnEnter onEnter { get; set; }
        public OnExecute onExecute { get; set; }
        public OnExit onExit { get; set; }

        [Inject] private PlayerTurnContainer _container;

        public void Enter()
        {
            SlotController slotController = _container.SlotController;
            slotController.Init();
            slotController.onSlotEvaluated += OnSlotEvaluated;
        }

        public void Execute()
        {
        }

        public void Exit()
        {
            SlotController slotController = _container.SlotController;
            slotController.onSlotEvaluated -= OnSlotEvaluated;
        }
        

        private async void OnSlotEvaluated(SlotEvaluateContainer resultContainer)
        {
            int swordCount = resultContainer.SwordCount;
            int magicCount = resultContainer.MagicCount;
            int attackCount = swordCount + magicCount;
            int shieldCount = resultContainer.ShieldCount;

            IncreaseShield(shieldCount);
            await AttackEnemy(attackCount);
        }

        private async UniTask AttackEnemy(int attackCount)
        {
            if (attackCount == 0)
                return;

            Player player = _container.Player;
            Enemy enemy = _container.Enemy;
            int attackPower = player.AttackPower;
            int damage = SlotResultCalculator.GetSlotAttackDamage(attackPower, attackCount);
            const float damageInterval = 0.1f;
            for (int i = 0; i < attackCount; i++)
            {
                enemy.OnDamaged(damage);   
                await UniTask.WaitForSeconds(damageInterval);
            }
        }

        private void IncreaseShield(int shieldCount)
        {
            if (shieldCount == 0)
                return;
            
            Player player = _container.Player;
            int shieldPower = player.ShieldPower;
            int shieldValue = SlotResultCalculator.GetSlotDefenseValue(shieldPower, shieldCount);
            player.IncreaseShield(shieldValue);
        }
    }

    public static class SlotResultCalculator
    {
        public static int GetSlotAttackDamage(int attackPower,int attackCount)
        {
            int damage = attackPower;
            float attackFactor = attackCount * 0.7f;

            damage = (int)(damage * attackFactor);
            return damage;
        }

        public static int GetSlotDefenseValue(int defensePower, int defenseCount)
        {
            int value = defensePower;
            float defenseFactor = defenseCount * 0.7f;

            value = (int)(value * defenseFactor);
            return value;
        }
    }
}