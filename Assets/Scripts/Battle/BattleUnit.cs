using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    public int HP
    {
        get => _hp;
        protected set
        {
            _hp = value;
            _hpImage.fillAmount = (float)_hp / _maxHP;
        }
    }

    private int _hp;
    [SerializeField] protected int _maxHP;
    [SerializeField] private Image _hpImage;

    public int Shield
    {
        get => _shield;
        protected set
        {
            _shield = value;
            if (_shieldImage != null && _maxShield > 0)
            {
                _shieldImage.fillAmount = (float)_shield / _maxShield;
            }
        }
    }

    private int _shield;
    [SerializeField] protected int _maxShield;
    [SerializeField] private Image _shieldImage;

    public virtual void Init()
    {
        _hp = _maxHP;
        HP = _hp;
        _shield = _maxShield;
        Shield = _shield;
    }

    public virtual void OnDamaged(int damage)
    {
        if (Shield != 0)
        {
            int originDamage = damage;
            damage -= Shield;
            int shieldDamage = originDamage - damage;
            DecreaseShield(shieldDamage);
        }

        HP -= damage;
        HP = Mathf.Clamp(HP, 0, HP);
    }

    public void IncreaseShield(int shieldValue)
    {
        if (_shield == 0)
        {
            _maxShield = 0;
        }
        _maxShield += shieldValue;
        _shield += shieldValue;
        Shield = _shield;
    }

    public void DecreaseShield(int shieldValue)
    {
        Shield -= shieldValue;
    }
}