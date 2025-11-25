using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    public int HP
    {
        get => _hp;
        protected set
        {
            _hp = value;
            _hpText.text = "체력: " + value;
        }
    }

    private int _hp;
    [SerializeField] protected int _maxHP;
    [SerializeField] private TextMeshProUGUI _hpText;
    
    public int Shield
    {
        get => _shield;
        protected set
        {
            _shield = value;
            _shieldText.text = "실드: " + value;
        }
    }

    private int _shield;
    [SerializeField] protected int _maxShield;
    [SerializeField] private TextMeshProUGUI _shieldText;

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
