using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Metroknight
{
    public class UIBossHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Image healthBarTrailingFill;
        [SerializeField] private TextMeshProUGUI bossTitle;
        [SerializeField] private float trailDelay = 0.4f;
        [SerializeField] private Enemy boss;
        private float currentBossHealth;

        public static UIBossHealthBar Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            healthBarFill.fillAmount = 1f;
            healthBarTrailingFill.fillAmount = 1f;
        }

        public void DisplayBossHealthBar(Enemy _boss, string _bossName)
        {
            bossTitle.text = _boss.enemyName ?? _bossName ?? "The Hollow Knight";
            boss = _boss;
            currentBossHealth = boss.health;
        }

        private void Update()
        {
            bossTitle.text = boss.enemyName ?? "The Hollow Knight";
            if (boss != null && boss.health != currentBossHealth)
            {
                currentBossHealth = boss.health;
                float _ratio = boss.health / boss.maxHealth;
                Sequence _sequence = DOTween.Sequence();
                _sequence
                    .Append(healthBarFill.DOFillAmount(_ratio, 0.25f))
                    .SetEase(Ease.InOutSine);
                _sequence.AppendInterval(trailDelay);
                _sequence
                    .Append(healthBarTrailingFill.DOFillAmount(_ratio, 0.3f))
                    .SetEase(Ease.InOutSine);

                _sequence.Play();
            }
        }

        private void OnDisable()
        {
            healthBarFill.fillAmount = 1f;
            healthBarTrailingFill.fillAmount = 1f;
            boss = null;
        }
    }
}
