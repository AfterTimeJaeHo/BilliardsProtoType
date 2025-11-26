using System;
using Aftertime.SecretSome.Common;
using Aftertime.SecretSome.Content;
using Aftertime.SecretSome.UI.Popup;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StateMachine.Runtime;
using UnityEngine;
using UnityEngine.UI;
using Waving.Battle;
using Waving.Di;
using Waving.Scene;
using Waving.UI;

namespace Waving.Content
{
    public class BattleContent : DIClass,IContent
    {
        public ContentState State { get; }
        public event Action<bool> onStop = delegate { };

        private static readonly Vector2 EnterCharacterStartPos = new Vector2(0, -300);
        private static readonly Vector2 EnterCharacterStartScale = new Vector2(0.8f, 0.8f);
        
        private BattleStateMachine stateMachine;
        private Action onEnterDirectingComplete;
        
        [Inject] private BattleContentContainer _container;

        public BattleContent()
        {
            // onEnterDirectingComplete += OnEnterDirectingComplete;
        }

        public async void StartContent()
        {
            await SceneEntryManager.Instance.Additive(GameScene.SlotPrototype);
            DIContainerBase.TryInjectAll(this);
            ResetView();
            
            stateMachine = new BattleStateMachine();
            stateMachine.Init(null);
            stateMachine.onAssaultExit += (isSuccess) =>
            {
                OnEnterDirectingComplete();
                // 적 체력 분기
                if (isSuccess)
                {
                    Enemy enemy = _container.Enemy;
                    int damage = (enemy.HP * 10) / 100;
                    enemy.OnDamaged(damage);
                }
            };
            
            // ShowEnterDirecting();
        }

        public void PauseContent()
        {
            UpdateExecutor.onUpdate -= stateMachine.Execute;
        }

        public UniTask StartContentAsync()
        {
            throw new System.NotImplementedException();
        }

        public UniTask StopContentAsync()
        {
            throw new System.NotImplementedException();
        }

        public async void StopContent()
        {
            UpdateExecutor.onUpdate -= stateMachine.Execute;
            ResetView();
            await SceneEntryManager.Instance.Remove(GameScene.SlotPrototype);
            PopupManager.Instance.Hide<FadePopup>();
            bool isClear = true;
            onStop.Invoke(isClear);
        }

        public void ResumeContent()
        {
            UpdateExecutor.onUpdate += stateMachine.Execute;
        }

        private void ShowEnterDirecting()
        {
            float BGFadeDuration = 0.2f;
            float CharacterShowDuration = 0.3f;
            float EnterCanvasGroupScaleDuration = 0.5f;
            float MainCanvasGroupScaleDuration = 0.2f;
            Image bgImage = _container.EnterBGImage;
            Image characterImage = _container.EnterCharacterImage;
            CanvasGroup enterCanvasGroup = _container.EnterCanvasGroup;
            CanvasGroup mainCanvasGroup = _container.MainCanvasGroup;

            Sequence sequence = DOTween.Sequence();
            enterCanvasGroup.alpha = 1;
            sequence.Append(bgImage.DOFade(1,BGFadeDuration));
            sequence.Append(characterImage.DOFade(1, CharacterShowDuration));
            sequence.Join(characterImage.rectTransform.DOAnchorPos(Vector2.zero, CharacterShowDuration));
            sequence.Join(characterImage.rectTransform.DOScale(Vector2.one, CharacterShowDuration));
            sequence.Append(enterCanvasGroup.transform.DOScale(1.2f, EnterCanvasGroupScaleDuration));
            sequence.Join(enterCanvasGroup.DOFade(0, EnterCanvasGroupScaleDuration));
            sequence.Append(mainCanvasGroup.DOFade(1, MainCanvasGroupScaleDuration));
            sequence.onComplete += () => onEnterDirectingComplete.Invoke();
        }
        
        // private void OnEnterDirectingComplete()
        // {
        //     CanvasGroup mainCanvasGroup = _container.MainCanvasGroup;
        //     mainCanvasGroup.interactable = true;
        //     mainCanvasGroup.blocksRaycasts = true;
        //     stateMachine = new BattleStateMachine();
        //     stateMachine.Init(null);
        //     UpdateExecutor.onUpdate += stateMachine.Execute;
        // }
        
        private void OnEnterDirectingComplete()
        {
            CanvasGroup mainCanvasGroup = _container.MainCanvasGroup;
            mainCanvasGroup.interactable = true;
            mainCanvasGroup.blocksRaycasts = true;
            UpdateExecutor.onUpdate += stateMachine.Execute;
        }

        private void ResetView()
        {
            CanvasGroup canvasGroup = _container.EnterCanvasGroup;
            Image bgImage = _container.EnterBGImage;
            Image characterImage = _container.EnterCharacterImage;
            Player player = _container.Player;
            Enemy enemy = _container.Enemy;
            
            canvasGroup.transform.localScale = Vector3.one;
            canvasGroup.alpha = 0;
            bgImage.color = new Color(1, 1, 1, 0);
            characterImage.color = new Color(1, 1, 1, 0);
            characterImage.rectTransform.anchoredPosition = EnterCharacterStartPos;
            characterImage.rectTransform.localScale = EnterCharacterStartScale;
            
            player.Init();
            enemy.Init();
        }
    }
    
    public enum BattleContentsState
    {
        Assault,
        PlayerTurn,
        EnemyTurn,
        HScene
    }
   
}