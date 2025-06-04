using UnityEngine;
using UnityEngine.SceneManagement;

namespace Metroknight {
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private string transitionTo;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Vector2 exitDirection;
        [SerializeField] private float exitTime;

        private AudioSource audioSource;
        [SerializeField] private AudioClip transitionSound;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (transitionTo == GameManager.Instance.transitionedFromScene)
            {
                PlayerController.Instance.transform.position = startPoint.position;
                // Fix weird inverted direction while changing scenes loading
                StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
            }
            StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        }


        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.CompareTag("Player"))
            {
                CheckShadeData();
                GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;
                audioSource.PlayOneShot(transitionSound);
                PlayerController.Instance.pState.cutscene = true;
                PlayerController.Instance.pState.invincible = true;
                StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
            }
        }

        void CheckShadeData()
        {
            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemyObjects)
            {
                if (enemy.GetComponent<Shade>() != null)
                {
                    SaveData.Instance.SaveShade();
                }
            }
        }
}
}
