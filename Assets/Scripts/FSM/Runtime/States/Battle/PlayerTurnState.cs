using System;
using Aftertime.MyTinyStreamer.Slots;
using Aftertime.SecretSome.UI.Popup;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SRPG;
using UnityEngine;
using UnityEngine.UI;
using Waving.Battle;
using Waving.Di;
using Waving.MyTinyStreamer.Common;
using Waving.UI;

namespace StateMachine.Runtime
{
    public class PlayerTurnState : DIClass, IState
    {
        public event Action onSlotEvaluated = delegate { };
        public event Action onEnemyDead = delegate { };
        
        public OnEnter onEnter { get; set; }
        public OnExecute onExecute { get; set; }
        public OnExit onExit { get; set; }

        [Inject] private PlayerTurnContainer _container;


        public async void Enter()
        {
            PopupManager.Instance.Push<TurnChangePopup>();
            TurnChangePopup turnChangePopup = PopupManager.Instance.GetPopup<TurnChangePopup>();
            await turnChangePopup.UpdatePlayerTurnView();
            
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
            slotController.SetInteractable(false);
        }
        

        private async void OnSlotEvaluated(SlotEvaluateContainer resultContainer)
        {
            int swordCount = resultContainer.SwordCount;
            int magicCount = resultContainer.MagicCount;
            int shieldCount = resultContainer.ShieldCount;

            IncreaseShield(shieldCount);
            await AttackEnemy(swordCount,magicCount);

            int enemyHP = _container.Enemy.HP;
            if (enemyHP <= 0)
            {
                onEnemyDead.Invoke();   
            }
            else
            {
                onSlotEvaluated.Invoke();   
            }
        }

        private async UniTask AttackEnemy(int swordCount, int magicCount)
        {
            if (swordCount == 0 && magicCount == 0)
                return;

            int attackCount = swordCount + magicCount;
            
            bool useSkill = swordCount == 3 || magicCount == 3;
            if (useSkill)
            {
                attackCount = 5;
                await PlaySkillDirecting();
            }

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

        private async UniTask PlaySkillDirecting()
        {
            Image bg = _container.PlayerSkillBG;
            Image character = _container.PlayerSkillCharacter;
            bg.rectTransform.localScale = Vector3.zero;
            bg.DOFade(0, 0);
            character.rectTransform.anchoredPosition = new Vector2(-25, -93);
            character.DOFade(0, 0);

            Material defaultMat = bg.material;
            bg.material = Resources.Load<Material>(Define.SkillBGMatPath);
            character.material = Resources.Load<Material>(Define.SkillCharacterMatPath);
            bg.material.mainTextureOffset = new Vector2(0, 0);
            character.material.mainTextureOffset = new Vector2(0, 0);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(bg.rectTransform.DOScale(Vector3.one, 0.2f));
            sequence.Join(bg.DOFade(1, 0.2f));
            sequence.Join(character.DOFade(1, 0.2f));
            sequence.AppendInterval(2);
            sequence.Append(bg.rectTransform.DOScale(new Vector3(1,0,1), 0.2f));
            sequence.Join(bg.DOFade(0, 0.2f));
            sequence.Join(character.DOFade(0, 0.2f));
            sequence.onComplete += () => sequence.Kill();
            await sequence.Play().ToUniTask();

            bg.material = defaultMat;
            character.material = defaultMat;
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