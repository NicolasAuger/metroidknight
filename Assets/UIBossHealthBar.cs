using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Metroknight
{
    public class UIBossHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBarContainer;
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TextMeshProUGUI bossTitle;
        [SerializeField] private Enemy boss;

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
            if (!healthBarFill)
            {
                Debug.LogError("Health Bar Fill Image is not assigned in the UIBossHealthBar script.");
            }

            healthBarContainer.gameObject.SetActive(false); // Hide the health bar initially
            healthBarFill.fillAmount = 1f; // Initialize the health bar to full
            healthBarFill.gameObject.SetActive(false); // Hide the health bar initially
            bossTitle.gameObject.SetActive(false); // Hide the boss title initially
        }

        public void DisplayBossHealthBar(Enemy _boss) {
            healthBarContainer.gameObject.SetActive(true);
            healthBarFill.gameObject.SetActive(true);
            bossTitle.gameObject.SetActive(true);
            bossTitle.text = _boss.enemyName ?? "The Hollow Knight";
            boss = _boss;
        }

        private void Update()
        {
            if (boss != null)
            {
                bossTitle.text = boss.enemyName ?? "The Hollow Knight";
                healthBarFill.fillAmount = boss.health / boss.maxHealth;
            }
        }
    }
}
