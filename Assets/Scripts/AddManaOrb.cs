using System.Collections;
using UnityEngine;

namespace Metroknight
{
    public class AddManaOrb : MonoBehaviour
    {
        bool used;
        [SerializeField] private GameObject unlockParticles;
        [SerializeField] private GameObject canvasUI;
        private float verticalOffset = 0.25f;
        private Vector2 initialPosition;
        private Rigidbody2D rb;
        private bool upwards = false;

        [SerializeField] OrbShard orbShards;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            if (PlayerController.Instance.manaOrbs >= 3)
            {
                Destroy(gameObject);
            }
            initialPosition = transform.position;
            upwards = true;
        }

        void Update()
        {
            CheckToHideCanvas();
            MoveVertically();
        }

        private void OnTriggerEnter2D(Collider2D _collision)
        {
            if (_collision.CompareTag("Player") && !used)
            {
                used = true;
                StartCoroutine(ShowCanvas());
            }
        }

        public void MoveVertically()
        {
            Vector2 _upperPosition = new Vector2(initialPosition.x, initialPosition.y + verticalOffset);
            Vector2 _lowerPosition = new Vector2(initialPosition.x, initialPosition.y - verticalOffset);

            if (upwards)
            {
                rb.MovePosition(Vector2.MoveTowards(transform.position, _upperPosition, 2 * Time.deltaTime));
                if (transform.position.y == _upperPosition.y)
                {
                    upwards = false;
                }
            }
            else
            {
                rb.MovePosition(Vector2.MoveTowards(transform.position, _lowerPosition, 2 * Time.deltaTime));
                if (transform.position.y == _lowerPosition.y)
                {
                    upwards = true;
                }
            }
        }

        IEnumerator ShowCanvas()
        {
            GameObject _particles = Instantiate(unlockParticles, transform.position, Quaternion.identity);
            PlayerController.Instance.transform.position = transform.position;
            Destroy(_particles, .75f);
            yield return new WaitForSecondsRealtime(0.75f);
            Time.timeScale = 0f; // Pause the game
            canvasUI.SetActive(true);

            orbShards.initialFillAmount = PlayerController.Instance.orbShards * .34f;
            PlayerController.Instance.orbShards++;
            orbShards.targetFillAmount = PlayerController.Instance.orbShards * .34f;

            StartCoroutine(orbShards.LerpFill());
        }

        private void CheckToHideCanvas()
        {
            if (Input.GetButtonDown("Escape") && canvasUI.activeSelf)
            {
                canvasUI.SetActive(false);
                Time.timeScale = 1f; // Resume the game
                Destroy(gameObject);
                SaveData.Instance.SavePlayer();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                new Vector2(initialPosition.x, initialPosition.y + verticalOffset),
                new Vector2(initialPosition.x, initialPosition.y - verticalOffset)
            );
        }
    }
}
