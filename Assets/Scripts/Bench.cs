using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroknight
{
    public class Bench : MonoBehaviour
    {
        public bool interacted;
        public bool playerInZone;
        public AudioSource audioSource;
        public AudioClip saveSound;
        public AudioClip mapDiscoveredSound;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.CompareTag("Player"))
            {
                playerInZone = true;
            }
        }

        private void OnTriggerExit2D(Collider2D _other)
        {
            if (_other.CompareTag("Player"))
            {
                playerInZone = false;
                interacted = false;
            }
        }

        private void Update()
        {
            if (Input.GetButtonDown("Interact") && playerInZone)
            {
                interacted = true;
                audioSource.PlayOneShot(saveSound);
                if (SaveData.Instance.scenesCount < SaveData.Instance.discoveredScenesCount)
                {
                    audioSource.clip = mapDiscoveredSound;
                    audioSource.PlayScheduled(AudioSettings.dspTime + saveSound.length);
                }
                PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
                SaveData.Instance.benchSceneName = SceneManager.GetActiveScene().name;
                SaveData.Instance.benchPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
                SaveData.Instance.SaveBench();
                SaveData.Instance.SavePlayer();
                SaveData.Instance.SaveMaps();
            }
        }
  }
}
