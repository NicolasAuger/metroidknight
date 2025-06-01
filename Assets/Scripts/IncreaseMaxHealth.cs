using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class IncreaseMaxHealth : MonoBehaviour
    {
        bool used;
        [SerializeField] private GameObject unlockParticles;
        [SerializeField] private GameObject canvasUI;
        private float verticalOffset = 0.25f;
        private Vector2 initialPosition;
        private Rigidbody2D rb;
        private bool upwards = false;

        [SerializeField] HeartShards heartshards;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            if (PlayerController.Instance.maxHealth >= PlayerController.Instance.maxTotalHealth)
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
            // PlayerController.Instance.transform.position = transform.position;
            Destroy(_particles, .75f);
            yield return new WaitForSecondsRealtime(0.75f);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Time.timeScale = 0f; // Pause the game
            canvasUI.SetActive(true);

            heartshards.initialFillAmount = PlayerController.Instance.heartShards * .25f;
            PlayerController.Instance.heartShards++;
            heartshards.targetFillAmount = PlayerController.Instance.heartShards * .25f;

            StartCoroutine(heartshards.LerpFill());
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