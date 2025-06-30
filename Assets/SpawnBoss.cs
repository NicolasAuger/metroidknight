using System.Collections;
using UnityEngine;

namespace Metroknight
{
    public class SpawnBoss : MonoBehaviour
    {
        public static SpawnBoss Instance;
        [SerializeField] Transform spawnPoint;
        [SerializeField] Enemy boss;
        [SerializeField] Vector2 exitDirection;
        [SerializeField] GameObject blockingFloor;
        [SerializeField] Transform blockingFloorTarget;
        [SerializeField] GameObject floor;
        BoxCollider2D col;
        BoxCollider2D floorCol;
        Transform originalBlockingFloorPosition;
        bool callOnce = false;
        bool shouldBlockEntry = false;
        float originalFloorColOffsetX;
        public bool isBossDead = false;

        private void Awake()
        {
            if (TheHollowKnight.Instance != null)
            {
                Destroy(TheHollowKnight.Instance);
                callOnce = false;
                col.isTrigger = true;
            }

            if (GameManager.Instance.THKDefeated)
            {
                callOnce = true;
            }

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
            col = GetComponent<BoxCollider2D>();
            originalBlockingFloorPosition = blockingFloor.transform;
            floorCol = floor.GetComponent<BoxCollider2D>();
            originalFloorColOffsetX = floorCol.offset.x;
        }

        private void FixedUpdate()
        {
            if (shouldBlockEntry & !isBossDead)
            {
                MoveBlockingFloor();
            }

            if (isBossDead)
            {
                shouldBlockEntry = false;
                RemoveBlockingFloor();
                UIManager.Instance.bossHealthBar.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.CompareTag("Player") && !callOnce && !GameManager.Instance.THKDefeated)
            {
                StartCoroutine(WalkIntoRoom());
                GameManager.Instance.EnteringBossFight();
                callOnce = true;

                // Remove trigger collider to prevent re-triggering
                col.enabled = false;
            }
        }

        IEnumerator WalkIntoRoom()
        {
            PlayerController.Instance.pState.cutscene = true;
            PlayerController.Instance.pState.invincible = true;
            PlayerController.Instance.rb.linearVelocity = Vector2.zero;
            PlayerController.Instance.xAxis = 0f;
            PlayerController.Instance.runningAudioSource.Stop();
            PlayerController.Instance.animator.SetBool("Walking", false);
            shouldBlockEntry = true;
            yield return new WaitForSeconds(3f);
            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, 1f));
            yield return new WaitForSeconds(1f);
            Enemy _boss = Instantiate(boss, spawnPoint.position, Quaternion.identity);
            UIManager.Instance.bossHealthBar.SetActive(true);
            UIBossHealthBar.Instance.DisplayBossHealthBar(_boss, "The Hollow Knight");
        }

        private void MoveBlockingFloor()
        {
            // Move the blocking floor to the target position
            Vector2 _newPos = Vector2.MoveTowards(
                blockingFloor.transform.position,
                new Vector2(blockingFloorTarget.position.x, blockingFloorTarget.position.y),
                1f * Time.fixedDeltaTime
            );
            blockingFloor.GetComponent<Rigidbody2D>().MovePosition(_newPos);
            floorCol.offset = new Vector2(8.5f, floorCol.offset.y);
        }

        private void RemoveBlockingFloor()
        {
            // Move the blocking floor to the target position
            Vector2 _newPos = Vector2.MoveTowards(
                blockingFloor.transform.position,
                new Vector2(originalBlockingFloorPosition.position.x, originalBlockingFloorPosition.position.y),
                1f * Time.fixedDeltaTime
            );
            blockingFloor.GetComponent<Rigidbody2D>().MovePosition(_newPos);
            floorCol.offset = new Vector2(originalFloorColOffsetX, floorCol.offset.y);
        }
    }
}