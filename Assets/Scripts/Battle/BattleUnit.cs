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
            if (_shieldImage != null)
            _shieldImage.fillAmount = (float)_shield / _maxShield;
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
        if(Shield != 0)
            damage -= Shield;
            
        HP -= damage;
        HP = Mathf.Clamp(HP,0, HP);
    }
}
