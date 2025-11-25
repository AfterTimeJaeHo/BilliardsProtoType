using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Waving.MyTinyStreamer.Common;

namespace Waving.Battle
{
    public class DamageText : MonoBehaviour
    {
        public TextMeshProUGUI Text => _text;
        [SerializeField] private TextMeshProUGUI _text;
        
        private static DamageText prefab;
        private const float FadeDuration = 0.2f;
        private const float Interval = 0.2f;
        private static Canvas battleCanvas;
        
        public static void ShowDamage(int damage)
        {
            if (prefab == null)
            {
                string path = Define.DamageTextPath;
                prefab = Resources.Load<DamageText>(path);
            }
            if (battleCanvas == null)
            {
                battleCanvas = GameObject.Find("BattleCanvas").GetComponent<Canvas>();
            }


            int randomX = Random.Range(-40, 40);
            int randomY = Random.Range(-40, 40);
            Vector2 spawnPos = new Vector2(randomX, randomY);
            RectTransform parent = battleCanvas.GetComponent<RectTransform>();
            DamageText damageText = Instantiate(prefab, Vector3.zero, Quaternion.identity,parent);
            damageText.GetComponent<RectTransform>().anchoredPosition = spawnPos;
            ShowText(damageText,damage);
        }

        private static void ShowText(DamageText damageText,int damage)
        {
            TextMeshProUGUI text = damageText.Text;
            Color originColor = text.color;
            text.color = new Color(originColor.r,originColor.g,originColor.b,0);
            text.SetText($"-{damage}");
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(text.DOFade(1, FadeDuration));
            sequence.AppendInterval(Interval);
            sequence.Append(text.DOFade(0, FadeDuration));
            sequence.onComplete += () => Destroy(damageText.gameObject);
        }
    }
   
}