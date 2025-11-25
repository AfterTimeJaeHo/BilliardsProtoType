using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Waving.Battle
{
    public class Enemy : BattleUnit
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        public override void OnDamaged(int damage)
        {
            base.OnDamaged(damage);
            DamageText.ShowDamage(damage);
        }
    }
   
}