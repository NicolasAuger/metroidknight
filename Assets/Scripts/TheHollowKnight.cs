using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class TheHollowKnight : Enemy
    {
        [SerializeField] GameObject slashEffect;
        public Transform sideAttackTransform, upAttackTransform, downAttackTransform;
        public Vector2 sideAttackArea, upAttackArea, downAttackArea;

        public float attackRange;
        public float attackTimer;


        [Header("Ground Check Settings")]
        [SerializeField] public Transform groundCheckPoint;
        [SerializeField] private float groundCheckY = 0.2f;
        [SerializeField] private float groundCheckX = 0.5f;
        [SerializeField] private LayerMask groundLayer;

        int hitCounter;
        bool stunned, canStun;
        bool alive;

        [HideInInspector] public float runSpeed;
        [HideInInspector] public bool facingRight;
        [HideInInspector] public bool attacking;
        [HideInInspector] public float attackCountdown;
        [HideInInspector] public bool damagedPlayer = false;
        [HideInInspector] public bool parrying;

        [HideInInspector] public Vector2 moveToPosition;
        [HideInInspector] public bool diveAttack;
        public GameObject divingCollider;
        public GameObject pillar;

        [HideInInspector] public bool barrageAttack;
        public GameObject barageFireball;

        [HideInInspector] public bool outbreakAttack;

        [HideInInspector] public bool bounceAttack;
        [HideInInspector] public float rotationDirectionToTarget;
        public int bounceCount;
        [HideInInspector] public GameObject groundShakeSmoke;
        int bounces = 0;


        public static TheHollowKnight Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(Instance.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        protected override void Start()
        {
            base.Start();
            sr = GetComponentInChildren<SpriteRenderer>();
            animator = GetComponentInChildren<Animator>();
            ChangeState(EnemyStates.THK_Stage1);
            alive = true;
        }

        protected override void Update()
        {
            base.Update();

            // Safe guard
            if (health <= 0 && alive)
            {
                Death(0f);
            }

            if (!attacking)
            {
                attackCountdown -= Time.deltaTime;
            }

            if (stunned)
            {
                rb.velocity = Vector2.zero;
            }
        }

        public void Flip()
        {
            if (PlayerController.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
            {
                transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
                facingRight = false;
            }
            else
            {
                transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
                facingRight = true;
            }
        }

        public bool Grounded()
        {
            if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer) ||
                Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckX, groundLayer) ||
                Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckX, groundLayer))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void UpdateEnemyStates()
        {
            if (PlayerController.Instance != null)
            {
                switch (GetCurrentEnemyState)
                {
                    case EnemyStates.THK_Stage1:
                        canStun = true;
                        attackTimer = 4f;
                        runSpeed = speed;
                        break;

                    case EnemyStates.THK_Stage2:
                        canStun = true;
                        attackTimer = 3f;
                        break;

                    case EnemyStates.THK_Stage3:
                        canStun = false;
                        attackTimer = 6f;
                        break;

                    case EnemyStates.THK_Stage4:
                        canStun = false;
                        attackTimer = 7f;
                        runSpeed = speed * .5f; // Slow down the boss in stage 4
                        break;
                }
            }
        }

        protected override void OnCollisionStay2D(Collision2D _other)
        {
            base.OnCollisionStay2D(_other);
        }

        #region Attacking

        public void AttackHandler()
        {
            if (currentEnemyState == EnemyStates.THK_Stage1)
            {
                if (Vector2.Distance(PlayerController.Instance.transform.position, transform.position) < attackRange)
                {
                    StartCoroutine(TripleSlash());
                }
                else
                {
                    StartCoroutine(Lunge());
                    // StartCoroutine(Lunge());
                    // DiveAttackJump();
                    // BarrageBendDown();
                    // OutbreakBendDown();
                    // BounceAttack();
                }
            }
            else if (currentEnemyState == EnemyStates.THK_Stage2)
            {
                if (Vector2.Distance(PlayerController.Instance.transform.position, transform.position) < attackRange)
                {
                    StartCoroutine(TripleSlash());
                }
                else
                {
                    int _attackChosen = Random.Range(1, 3);
                    if (_attackChosen == 1)
                    {
                        StartCoroutine(Lunge());
                    }
                    else if (_attackChosen == 2)
                    {
                        DiveAttackJump();
                    }
                    else if (_attackChosen == 3)
                    {
                        BarrageBendDown();
                    }
                }
            }
            else if (currentEnemyState == EnemyStates.THK_Stage3)
            {
                int _attackChosen = Random.Range(1, 4);
                if (_attackChosen == 1)
                {
                    OutbreakBendDown();
                }
                else if (_attackChosen == 2)
                {
                    DiveAttackJump();
                }
                else if (_attackChosen == 3)
                {
                    BarrageBendDown();
                } else if (_attackChosen == 4)
                {
                    BounceAttack();
                }
            }
            else if (currentEnemyState == EnemyStates.THK_Stage4)
            {
                if (currentEnemyState == EnemyStates.THK_Stage1)
                {
                    if (Vector2.Distance(PlayerController.Instance.transform.position, transform.position) < attackRange)
                    {
                        StartCoroutine(Slash());
                    }
                    else
                    {
                        BounceAttack();
                    }
                }
            }
        }

        public void ResetAllAttacks()
        {
            attacking = false;
            StopCoroutine(TripleSlash());
            StopCoroutine(Lunge());
            StopCoroutine(Parry());
            StopCoroutine(Slash());
            diveAttack = false;
            barrageAttack = false;
            outbreakAttack = false;
            bounceAttack = false;
        }

        IEnumerator TripleSlash()
        {
            attacking = true;
            rb.velocity = Vector2.zero;

            animator.SetTrigger("Slash");
            SlashAngle();
            yield return new WaitForSeconds(0.3f);
            animator.ResetTrigger("Slash");

            animator.SetTrigger("Slash");
            SlashAngle();
            yield return new WaitForSeconds(0.5f);
            animator.ResetTrigger("Slash");

            animator.SetTrigger("Slash");
            SlashAngle();
            yield return new WaitForSeconds(0.2f);
            animator.ResetTrigger("Slash");

            ResetAllAttacks();
        }

        void SlashAngle()
        {
            if (PlayerController.Instance.transform.position.x > transform.position.x ||
                PlayerController.Instance.transform.position.x < transform.position.x)
            {
                Instantiate(slashEffect, sideAttackTransform);
            }
            else if (PlayerController.Instance.transform.position.y > transform.position.y)
            {
                SlashEffectAtAngle(slashEffect, 80, upAttackTransform);
            }
            else if (PlayerController.Instance.transform.position.y < transform.position.y)
            {
                SlashEffectAtAngle(slashEffect, -90, downAttackTransform);
            }
        }

        void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
        {
            _slashEffect = Instantiate(_slashEffect, _attackTransform);
            _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
            _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
        }

        IEnumerator Lunge()
        {
            Flip();
            attacking = true;
            animator.SetBool("Lunge", true);
            yield return new WaitForSeconds(1f);
            animator.SetBool("Lunge", false);
            damagedPlayer = false;
            ResetAllAttacks();
        }

        IEnumerator Parry()
        {
            parrying = true;
            rb.velocity = Vector2.zero;
            animator.SetBool("Parry", true);
            // Parry lasts 0.75 seconds, so we set 0.8 over there
            yield return new WaitForSeconds(0.8f);
            animator.SetBool("Parry", false);
            parrying = false;
            ResetAllAttacks();
        }

        IEnumerator Slash()
        {
            attacking = true;
            rb.velocity = Vector2.zero;

            animator.SetTrigger("Slash");
            SlashAngle();
            yield return new WaitForSeconds(0.2f);
            animator.ResetTrigger("Slash");

            ResetAllAttacks();
        }

        #region Stage2

        public void DiveAttackJump()
        {
            attacking = true;
            moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
            diveAttack = true;
            animator.SetBool("Jump", true);
        }

        public void Dive()
        {
            animator.SetBool("Dive", true);
            animator.SetBool("Jump", false);
        }

        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.GetComponent<PlayerController>() != null && (diveAttack || bounceAttack))
            {
                _other.GetComponent<PlayerController>().TakeDamage(damage * 2);
                PlayerController.Instance.pState.recoilingX = true;
            }
        }

        public void DivingPillars()
        {
            Vector2 _impactPoint = groundCheckPoint.position;
            float _spawnDistance = 5;
            for (int i = 0; i < 10; i++)
            {
                Vector2 _pillarSpawnPointRight = _impactPoint + new Vector2(_spawnDistance, 0);
                Vector2 _pillarSpawnPointLeft = _impactPoint - new Vector2(_spawnDistance, 0);
                Instantiate(pillar, _pillarSpawnPointRight, Quaternion.Euler(0, 0, -90));
                Instantiate(pillar, _pillarSpawnPointLeft, Quaternion.Euler(0, 0, -90));

                _spawnDistance += 5f; // Increase distance for each pillar
            }
            ResetAllAttacks();
        }

        void BarrageBendDown()
        {
            attacking = true;
            rb.velocity = Vector2.zero;
            barrageAttack = true;
            animator.SetTrigger("BendDown");
        }

        public IEnumerator Barrage()
        {
            rb.velocity = Vector2.zero;
            float _currentAngle = 30f; // Starting angle for the barrage
            for (int i = 0; i < 10; i++)
            {
                GameObject _projectile = Instantiate(barageFireball, transform.position, Quaternion.Euler(0, 0, _currentAngle));

                if (facingRight)
                {
                    _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 0, _currentAngle);
                }
                else
                {
                    _projectile.transform.eulerAngles = new Vector3(_projectile.transform.eulerAngles.x, 180, _currentAngle);
                }

                _currentAngle += 5f;
                yield return new WaitForSeconds(0.4f); // Delay between each projectile
            }
            yield return new WaitForSeconds(0.1f); // Wait for the barrage to finish
            animator.SetBool("Cast", false);
            ResetAllAttacks();
        }

        #endregion

        #region Stage3

        public void OutbreakBendDown()
        {
            attacking = true;
            rb.velocity = Vector2.zero;
            moveToPosition = new Vector2(transform.position.x, rb.position.y + 5);
            outbreakAttack = true;
            animator.SetTrigger("BendDown");
        }

        public IEnumerator Outbreak()
        {
            yield return new WaitForSeconds(1f); // Wait for the bend down animation to finish
            animator.SetBool("Cast", true);
            rb.velocity = Vector2.zero;

            for (int i = 0; i < 30; i++)
            {
                Instantiate(barageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(110, 130))); // Downward random angle
                Instantiate(barageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(50, 70))); // Diagonal right angle
                Instantiate(barageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(260, 280))); // Diagonal left angle

                yield return new WaitForSeconds(0.2f); // Delay between each projectile
            }
            yield return new WaitForSeconds(0.1f); // Wait for the barrage to finish
            rb.constraints = RigidbodyConstraints2D.None; // Unfreeze position
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze rotation
            rb.velocity = new Vector2(rb.velocity.x, -10); // Reset velocity to fall down
            yield return new WaitForSeconds(0.1f); // Wait for the fall down
            animator.SetBool("Cast", false);
            ResetAllAttacks();
        }

        void BounceAttack()
        {
            attacking = true;
            bounceCount = Random.Range(2, 5);
            BounceBendDown();
        }

        public void BounceBendDown()
        {
            rb.velocity = Vector2.zero;
            moveToPosition = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y + 10);
            bounceAttack = true;
            animator.SetTrigger("BendDown");
        }

        public void CalculcateTargetAngle()
        {
            Vector3 _directionToTarget = (PlayerController.Instance.transform.position - transform.position).normalized;
            float _angleOfTarget = Mathf.Atan2(_directionToTarget.y, _directionToTarget.x) * Mathf.Rad2Deg;
            rotationDirectionToTarget = _angleOfTarget;
        }

        public void CheckBounce()
        {
            if (bounces < bounceCount - 1)
            {
                bounces++;
                BounceBendDown();
            }
            else
            {
                bounces = 0;
                // Force boss to run after bouncing the required number of times
                animator.Play("Boss_Run");
            }
        }

        #endregion
        #endregion

        public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
        {
            if (!stunned)
            {
                if (!parrying)
                {
                    if (canStun)
                    {
                        hitCounter++;
                        if (hitCounter >= 3)
                        {
                            ResetAllAttacks();
                            StartCoroutine(Stunned());
                        }
                    }
                    base.EnemyHit(_damageDone, _hitDirection, _hitForce);
                    if (currentEnemyState != EnemyStates.THK_Stage4)
                    {
                        ResetAllAttacks(); // cancel any current attack to avoid bugs
                        StartCoroutine(Parry());
                    }
                }
                else
                {
                    StopCoroutine(Parry());
                    ResetAllAttacks();
                    StartCoroutine(Slash()); // Riposte
                }
            }
            else
            {
                StopCoroutine(Stunned());
                animator.SetBool("Stunned", false);
                stunned = false;
            }

            #region Health to state
            if (health > 20)
            {
                ChangeState(EnemyStates.THK_Stage1);
            }
            else if (health <= 20 && health > 15)
            {
                ChangeState(EnemyStates.THK_Stage2);
            }
            else if (health <= 15 && health > 10)
            {
                ChangeState(EnemyStates.THK_Stage3);
            }
            else if (health <= 10 && health > 0)
            {
                ChangeState(EnemyStates.THK_Stage4);
            }
            else if (health <= 0)
            {
                Death(0f);
            }
            #endregion
        }

        public IEnumerator Stunned()
        {
            stunned = true;
            hitCounter = 0;
            animator.SetBool("Stunned", true);
            yield return new WaitForSeconds(3f); // Stunned for 3 second
            animator.SetBool("Stunned", false);
            stunned = false;
        }

        protected override void Death(float _destroyTime)
        {
            ResetAllAttacks();
            alive = false;
            rb.velocity = new Vector2(rb.velocity.x, -25);
            animator.SetTrigger("Die");
        }

        public void DestroyAfterDeath()
        {
            Destroy(gameObject);
            SpawnBoss.Instance.isBossDead = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            // Draw basic slash areas
            Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
            Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
            Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);
            Gizmos.color = Color.green;

            // Draw diving collider before pillar spawning
            Gizmos.DrawWireSphere(
                divingCollider.transform.position + new Vector3(0, divingCollider.GetComponent<CircleCollider2D>().offset.y),
                divingCollider.GetComponent<CircleCollider2D>().radius
            );

            // Draw ground check points
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(groundCheckPoint.position, Vector2.down * groundCheckY);
        }
    }
}