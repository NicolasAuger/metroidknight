using System.Collections;
using UnityEngine;

namespace Metroknight
{
    public class UnlockWallJump : MonoBehaviour
    {
        bool used;
        [SerializeField] private GameObject unlockParticles;
        [SerializeField] private GameObject canvasUI;
        private AudioSource audioSource;
        [SerializeField] private AudioClip unlockSound;

        void Start()
        {
            if (PlayerController.Instance.unlockedWallJump)
            {
                Destroy(gameObject);
            }
            audioSource = GetComponent<AudioSource>();
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
                audioSource.PlayOneShot(unlockSound);
                StartCoroutine(ShowCanvas());
            }
        }

        IEnumerator ShowCanvas()
        {
            GameObject _particles = Instantiate(unlockParticles, transform.position, Quaternion.identity);
            Destroy(_particles, .5f);
            yield return new WaitForSeconds(0.5f);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            canvasUI.SetActive(true);
            PlayerController.Instance.unlockedWallJump = true;
            SaveData.Instance.SavePlayer();
        }

        private void CheckToHideCanvas()
        {
            if (Input.GetKeyDown(KeyCode.X) && canvasUI.activeSelf)
            {
                canvasUI.SetActive(false);
                Destroy(gameObject);
            }
        }
    }
}
