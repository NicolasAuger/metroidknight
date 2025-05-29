using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class UnlockWallJump : MonoBehaviour
    {
        bool used;
        [SerializeField] private GameObject unlockParticles;
        [SerializeField] private GameObject canvasUI;

        void Start()
        {
            if (PlayerController.Instance.unlockedWallJump)
            {
                Destroy(gameObject);
            }
        }

    void Update()
    {
        CheckToHideCanvas();
    }

    private void OnTriggerEnter2D(Collider2D _collision)
        {
            if (_collision.CompareTag("Player") && !used)
            {
                used = true;
                StartCoroutine(ShowCanvas())
;            }
        }

        IEnumerator ShowCanvas()
        {
            GameObject _particles = Instantiate(unlockParticles, transform.position, Quaternion.identity);
            Destroy(_particles, .5f);
            yield return new WaitForSeconds(0.5f);
            canvasUI.SetActive(true);
            PlayerController.Instance.unlockedWallJump = true;
            SaveData.Instance.SavePlayer();
        }

        private void CheckToHideCanvas()
        {
            if (Input.GetButtonDown("Escape") && canvasUI.activeSelf)
            {
                canvasUI.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}
