using System;
using Aftertime.SecretSome.UI.Popup;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SRPG;
using UnityEngine;
using Waving.Battle;
using Waving.Di;
using Waving.UI;

namespace StateMachine.Runtime {
    public class EnemyTurnState : DIClass,IState 
    {
        public event Action onAttack;
        
        public OnEnter onEnter { get; set; }
        public OnExecute onExecute { get; set; }
        public OnExit onExit { get; set; }
        
        [Inject] private EnemyTurnContainer _container;
        
        public async void Enter()
        {
            TurnChangePopup turnChangePopup = PopupManager.Instance.GetPopup<TurnChangePopup>();
            await turnChangePopup.UpdateEnemyTurnView();
            await AttackPlayer();
            onAttack.Invoke();
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }

        private async UniTask AttackPlayer()
        {
            Player player = _container.Player;
            Enemy enemy = _container.Enemy;
            RectTransform stageHolder = _container.StageHolder;
            int attackPower = enemy.AttackPower;
            player.OnDamaged(attackPower);

            stageHolder.DOShakeAnchorPos(
                duration: 0.5f,
                strength: new Vector2(10f, 0f), // X축 20px shake
                vibrato: 10, // 흔들림 주기
                randomness: 0f // 랜덤 흔들림 최소화
            );
            
            const float damageInterval = 0.6f;
            await UniTask.WaitForSeconds(damageInterval);
        }
    }
}