using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Waving.Battle
{
    public class Player : BattleUnit
    {
        public readonly int AttackPower = 10;
        public readonly int ShieldPower = 5;
        
        public override void OnDamaged(int damage)
        {
            base.OnDamaged(damage);
            DamageText.ShowDamage(damage);
        }
    }
   
}