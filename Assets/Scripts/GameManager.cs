using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroknight
{
    public class GameManager : MonoBehaviour
    {
        public string transitionedFromScene;
        public Vector2 platformingRespawnPoint;
        public Vector2 respawnPoint;
        [SerializeField] Bench bench;
        public GameObject shade;

        // Singleton instance
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            SaveData.Instance.Initialize();

            // Check if instance already exists
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Destroy duplicate instance
            }
            else
            {
                Instance = this;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            DontDestroyOnLoad(gameObject); // Persist across scenes
            bench = FindObjectOfType<Bench>();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SaveScene();
        }

        public void RespawnPlayer()
        {
            if (bench != null && bench.interacted)
            {
                respawnPoint = bench.transform.position;
            }
            else
            {
                respawnPoint = platformingRespawnPoint;
            }
            PlayerController.Instance.transform.position = respawnPoint;
            StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
            PlayerController.Instance.Respawned();
        }

        public void SaveScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SaveData.Instance.sceneNames.Add(currentSceneName);
        }
    }
}
