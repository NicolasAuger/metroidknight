using UnityEngine;
using UnityEngine.UI;

namespace Metroknight {
    public class HeartController : MonoBehaviour
    {

        PlayerController player;

        private GameObject[] heartContainers;
        private Image[] heartFills;
        public Transform heartsParent;
        public GameObject heartContainerPrefab;

        void SetHeartsContainers() {
            for (int i = 0; i < heartContainers.Length; i++) {
                if (i < PlayerController.Instance.maxHealth) {
                    heartContainers[i].SetActive(true);
                } else {
                    heartContainers[i].SetActive(false);
                }
            }
        }

        void SetFilledHearts() {
            for (int i = 0; i < heartFills.Length; i++) {
                if (i < PlayerController.Instance.Health) {
                    heartFills[i].fillAmount = 1;
                } else {
                    heartFills[i].fillAmount = 0;
                }
            }
        }

        public void InstantiateHeartContainers()
        {
            heartContainers = new GameObject[PlayerController.Instance.maxTotalHealth];
            heartFills = new Image[PlayerController.Instance.maxTotalHealth];

            for (int i = 0; i < PlayerController.Instance.maxTotalHealth; i++)
            {
                GameObject temp = Instantiate(heartContainerPrefab);
                temp.transform.SetParent(heartsParent, false);
                heartContainers[i] = temp;
                heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
            }
            
            PlayerController.Instance.onHealthChangedCallback += UpdateHeartHUD;
            UpdateHeartHUD();
        }

        void UpdateHeartHUD() {
            SetHeartsContainers();
            SetFilledHearts();
        }
    }
}
