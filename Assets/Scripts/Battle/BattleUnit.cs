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

    [SerializeField] private int _hp;
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

    [SerializeField] private int _shield;
    [SerializeField] private TextMeshProUGUI _shieldText;

    public void Init()
    {
        HP = _hp;
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
