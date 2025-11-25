using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Waving.Battle
{
    public class Enemy : BattleUnit
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private Image _overlayImage;

        [Header("[Armor Sprite]")] 
        [SerializeField] private Sprite _armorSprite1;
        [SerializeField] private Sprite _armorSprite2;
        [SerializeField] private Sprite _armorSprite3;
        [SerializeField] private Sprite _armorSprite4;
        [SerializeField] private Sprite _damageArmorSprite1;
        [SerializeField] private Sprite _damageArmorSprite2;
        [SerializeField] private Sprite _damageArmorSprite3;
        [SerializeField] private Sprite _damageArmorSprite4;

        public readonly int AttackPower = 20;
        public readonly int ShieldPower = 0;
        
        private const float StandingChangeDuration = 0.2f;
        private Sequence _damagedSequence;

        private AmorState _amorState;

        public override void Init()
        {
            base.Init();

            _amorState = AmorState.Level1;
            _overlayImage.DOKill();
            _image.DOKill();
            _overlayImage.color = new Color(1, 1, 1, 0);
            _image.color = Color.white;
            _image.sprite = _armorSprite1;
        }

        public override void OnDamaged(int damage)
        {
            base.OnDamaged(damage);
            DamageText.ShowDamage(damage);
            UpdateBreakState();
            PlayDamageDirecting();
        }

        private void PlayDamageDirecting()
        {
            UpdateDamagedStanding();
            
            if(_damagedSequence != null)
                _damagedSequence.Kill();
            
            _damagedSequence = DOTween.Sequence();
            _damagedSequence.Append(_rectTransform.DOShakeAnchorPos(
                duration: 0.5f,
                strength: new Vector2(20f, 0f), // X축 20px shake
                vibrato: 10, // 흔들림 주기
                randomness: 0f // 랜덤 흔들림 최소화
            ));
            _damagedSequence.onComplete += UpdateIdleStanding;
        }


        private void UpdateIdleStanding()
        {
            Sprite changeSprite = GetIdleSprite(_amorState);
            UpdateStanding(changeSprite);
        }

        private void UpdateDamagedStanding()
        {
            Sprite changeSprite = GetDamagedSprite(_amorState);
            UpdateStanding(changeSprite);
        }

        private void UpdateStanding(Sprite changeSprite)
        {
            _image.DOKill();
            _overlayImage.DOKill();
            
            _overlayImage.sprite = changeSprite;
            _overlayImage.DOFade(1, StandingChangeDuration).onComplete += () =>
            {
                _image.sprite = changeSprite;
                _overlayImage.sprite = null;
                _overlayImage.color = new Color(1, 1, 1, 0);
            };
        }

        private void UpdateBreakState()
        {
            int armorBreakLevel2 = (_maxHP / 100) * 90;
            int armorBreakLevel3 = (_maxHP / 100) * 60;
            int armorBreakLevel4 = (_maxHP / 100) * 30;

            if (HP <= armorBreakLevel4)
            {
                _amorState = AmorState.Level4;
            }
            else if (HP <= armorBreakLevel3)
            {
                _amorState = AmorState.Level3;
            }
            else if (HP <= armorBreakLevel2)
            {
                _amorState = AmorState.Level2;
            }
            else
            {
                _amorState = AmorState.Level1;
            }
        }

        private Sprite GetDamagedSprite(AmorState amorState)
        {
            if (amorState == AmorState.Level4)
            {
                return _damageArmorSprite4;
            }
            else if (amorState == AmorState.Level3)
            {
                return _damageArmorSprite3;
            }
            else if (amorState == AmorState.Level2)
            {
                return _damageArmorSprite2;
            }
            else
            {
                return _damageArmorSprite1;
            }
        }

        private Sprite GetIdleSprite(AmorState amorState)
        {
            if (amorState == AmorState.Level4)
            {
                return _armorSprite4;
            }
            else if (amorState == AmorState.Level3)
            {
                return _armorSprite3;
            }
            else if (amorState == AmorState.Level2)
            {
                return _armorSprite2;
            }
            else
            {
                return _armorSprite1;
            }
        }
    }

    public enum AmorState
    {
        Level1,
        Level2,
        Level3,
        Level4,
    }
}