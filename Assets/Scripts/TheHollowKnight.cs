using System.Collections;
using UnityEngine;

namespace Metroknight
{
    public class TheHollowKnight : Enemy
    {
        bool alive;

        [Header("Ground Check Settings")]
        [SerializeField] public Transform groundCheckPoint;
        [SerializeField] public Transform wallCheckPoint;
        [SerializeField] private float groundCheckY = 0.2f;
        [SerializeField] private float groundCheckX = 0.5f;
        [SerializeField] private LayerMask groundLayer;
        [HideInInspector] public GameObject groundShakeSmoke;
        [Space(5)]

        [Header("Run Settings")]
        public float bufferZone = 1f; // Hysteresis (tampons zone to prevent boss jittering between idle & run)
        [HideInInspector] public float runSpeed;
        [HideInInspector] public bool facingRight;
        [Space(5)]

        [Header("Attack Settings")]
        [SerializeField] GameObject slashEffect;
        public Transform sideAttackTransform, upAttackTransform, downAttackTransform;
        public Vector2 sideAttackArea, upAttackArea, downAttackArea;
        public float attackRange;
        public float attackTimer;
        [HideInInspector] public bool attacking;
        [HideInInspector] public bool damagedPlayer = false;
        [HideInInspector] public float attackCountdown;
        private Coroutine tripleSlashCoroutine;
        [Space(5)]

        [Header("Stun Settings")]
        bool stunned, canStun;
        int hitCounter;
        [Space(5)]

        [Header("Parry Settings")]
        [HideInInspector] public bool parrying;
        private Coroutine parryCoroutine;
        [Space(5)]

        [Header("Particles")]
        [SerializeField] ParticleSystem outbreakParticles;
        [SerializeField] ParticleSystem parryParticles;
        [SerializeField] ParticleSystem tripleSlashParticles;
        [Space(10)]

        [Header("----- SPECIAL ATTACKS -----")]
        [Space(10)]

        [Header("Lunge Attack Settings")]
        public bool lunging = false;
        [Space(5)]

        [Header("Diving Attack Settings")]
        public Vector2 moveToPosition;
        public GameObject divingCollider;
        public GameObject pillar;
        [SerializeField] private AudioClip divingPillarSound;
        [HideInInspector] public bool diveAttack;
        [Space(5)]

        [Header("Barrage Attack Settings")]
        public GameObject barageFireball;
        [SerializeField] private AudioClip fireBallSound;
        [HideInInspector] public bool barrageAttack;
        [Space(5)]

        [Header("Bounce Attack Settings")]
        public int bounceCount;
        int bounces = 0;
        [HideInInspector] public bool bounceAttack;
        [HideInInspector] public float rotationDirectionToTarget;
        [Space(5)]

        [Header("Outbreak Attack Settings")]
        [HideInInspector] public bool outbreakAttack;
        [Space(5)]

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
            damageFlash = GetComponentInChildren<DamageFlash>();
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponent<AudioSource>();
            outbreakParticles.Stop();
            enemyName = "The HollowKnight";

            ChangeState(EnemyStates.THK_Stage1);
            alive = true;
            Flip();
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
                rb.linearVelocity = Vector2.zero;
            }
        }

        protected void FixedUpdate()
        {
            if (attacking || lunging) return;
            Flip();
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

        #region Enemy States
        protected override void UpdateEnemyStates()
        {
            if (PlayerController.Instance != null)
            {
                switch (GetCurrentEnemyState)
                {
                    case EnemyStates.THK_Stage1:
                        canStun = true;
                        attackTimer = 1.5f;
                        runSpeed = speed;
                        break;

                    case EnemyStates.THK_Stage2:
                        canStun = true;
                        attackTimer = 1f;
                        break;

                    case EnemyStates.THK_Stage3:
                        canStun = false;
                        attackTimer = 2f;
                        break;

                    case EnemyStates.THK_Stage4:
                        canStun = false;
                        attackTimer = 2.5f;
                        runSpeed = speed * .5f; // Slow down the boss in stage 4
                        break;
                }
            }
        }
        #endregion

        protected override void OnCollisionStay2D(Collision2D _other)
        {
            base.OnCollisionStay2D(_other);
        }

        #region Boss Attacks per stage

        public void AttackHandler()
        {
            if (currentEnemyState == EnemyStates.THK_Stage1)
            {
                if (Vector2.Distance(PlayerController.Instance.transform.position, transform.position) < attackRange)
                {
                    tripleSlashCoroutine = StartCoroutine(TripleSlash());
                }
                else
                {
                    StartCoroutine(Lunge());

                    // Uncomment to test quickly every attacks
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
                    tripleSlashCoroutine = StartCoroutine(TripleSlash());
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
                }
                else if (_attackChosen == 4)
                {
                    BounceAttack();
                }
            }
            else if (currentEnemyState == EnemyStates.THK_Stage4)
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

        public void ResetAllAttacks()
        {
            attacking = false;
            if (tripleSlashCoroutine != null) StopCoroutine(tripleSlashCoroutine);
            StopCoroutine(Lunge());
            StopCoroutine(Parry());
            StopCoroutine(Slash());
            diveAttack = false;
            barrageAttack = false;
            outbreakAttack = false;
            bounceAttack = false;
            lunging = false;

            // Try fixing jump looping animation when hit while stunned
            animator.SetBool("Jump", false);
            animator.SetBool("Cast", false);
            animator.SetBool("BendDown", false);
            animator.SetBool("Dive", false);
            animator.SetBool("Lunge", false);
            animator.ResetTrigger("Slash");
            animator.ResetTrigger("BendDown");
            animator.ResetTrigger("Bounce1");
            animator.ResetTrigger("Bounce2");
        }

        IEnumerator TripleSlash()
        {
            attacking = true;
            rb.linearVelocity = Vector2.zero;

            if (tripleSlashParticles != null)
            {
                tripleSlashParticles.gameObject.SetActive(true);
                tripleSlashParticles.Play();
            }

            yield return new WaitForSeconds(tripleSlashParticles.main.duration);
            tripleSlashParticles.Stop();
            tripleSlashParticles.gameObject.SetActive(false);

            animator.SetTrigger("Slash");
            rb.AddForce(new Vector2(facingRight ? 20 : -20, 0), ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
            rb.linearVelocityX = 0;
            yield return new WaitForSeconds(0.3f);
            animator.ResetTrigger("Slash");

            animator.SetTrigger("Slash");
            rb.AddForce(new Vector2(facingRight ? 30 : -30, 0), ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.15f);
            rb.linearVelocityX = 0;
            yield return new WaitForSeconds(0.5f);
            animator.ResetTrigger("Slash");

            animator.SetTrigger("Slash");
            rb.AddForce(new Vector2(facingRight ? 20 : -20, 0), ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.2f);
            rb.linearVelocityX = 0;
            yield return new WaitForSeconds(0.1f);
            animator.ResetTrigger("Slash");

            // Wait for the last slash animation to finish
            yield return new WaitForSeconds(0.5f);

            rb.linearVelocityX = 0;
            ResetAllAttacks();
        }

        public void SlashAngle()
        {
            Instantiate(slashEffect, sideAttackTransform);
        }

        IEnumerator Lunge()
        {
            attacking = true;
            lunging = true;
            animator.SetTrigger("LoadingLunge");
            yield return null;
        }

        IEnumerator Parry()
        {
            // Parry only one out of 4 times
            int _randomParry = Random.Range(1, 4);

            if (_randomParry == 1)
            {
                rb.linearVelocity = Vector2.zero;
                parrying = true;
                animator.SetTrigger("QuickParry");
                yield return new WaitForSeconds(1);
                parrying = false;
                ResetAllAttacks();
            }

            yield return null;
        }

        IEnumerator Slash()
        {
            attacking = true;
            rb.linearVelocity = Vector2.zero;
            animator.SetTrigger("Slash");
            yield return new WaitForSeconds(0.4f);
            animator.ResetTrigger("Slash");
            attacking = false;
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

                // Increase distance for each pillar
                _spawnDistance += 5f;
            }
            audioSource.PlayOneShot(divingPillarSound, .7f);
            ResetAllAttacks();
        }

        void BarrageBendDown()
        {
            attacking = true;
            rb.linearVelocity = Vector2.zero;
            barrageAttack = true;
            animator.SetTrigger("BendDown");
        }

        public IEnumerator Barrage()
        {
            rb.linearVelocity = Vector2.zero;
            float _currentAngle = 30f; // Starting angle for the barrage
            if (outbreakParticles != null)
            {
                outbreakParticles.gameObject.SetActive(true);
                outbreakParticles.Play();
            }
            for (int i = 0; i < 10; i++)
            {
                GameObject _projectile = Instantiate(barageFireball, transform.position, Quaternion.Euler(0, 0, _currentAngle));
                audioSource.PlayOneShot(fireBallSound, .15f);

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
            outbreakParticles.Stop();
            ResetAllAttacks();
        }
        #endregion

        #region Stage3
        public void OutbreakBendDown()
        {
            attacking = true;
            rb.linearVelocity = Vector2.zero;
            moveToPosition = new Vector2(transform.position.x, rb.position.y + 5);
            outbreakAttack = true;
            animator.SetTrigger("BendDown");
        }

        public IEnumerator Outbreak()
        {
            yield return new WaitForSeconds(1f); // Wait for the bend down animation to finish
            animator.SetBool("Cast", true);
            rb.linearVelocity = Vector2.zero;

            if (outbreakParticles != null)
            {
                outbreakParticles.gameObject.SetActive(true);
                outbreakParticles.Play();
            }

            for (int i = 0; i < 30; i++)
            {
                Instantiate(barageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(100, 160))); // Downward random angle
                Instantiate(barageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(20, 80))); // Diagonal right angle
                Instantiate(barageFireball, transform.position, Quaternion.Euler(0, 0, Random.Range(240, 300))); // Diagonal left angle
                audioSource.PlayOneShot(fireBallSound, .15f);
                yield return new WaitForSeconds(0.2f); // Delay between each projectile
            }
            outbreakParticles.Stop();
            yield return new WaitForSeconds(0.1f); // Wait for the barrage to finish
            rb.constraints = RigidbodyConstraints2D.None; // Unfreeze position
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Freeze rotation
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -10); // Reset velocity to fall down
            yield return new WaitForSeconds(0.1f); // Wait for the fall down
            outbreakParticles.gameObject.SetActive(false);
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
            rb.linearVelocity = Vector2.zero;
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
                // Force boss to idle after bouncing the required number of times
                animator.Play("Boss_Idle");
            }
        }
        #endregion
        #endregion

        #region Hit
        public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
        {
            // Reset stun effect on boss while being hit again
            if (stunned)
            {
                StopCoroutine(Stunned());
                animator.SetBool("Stunned", false);
                stunned = false;
            }

            // If hit while parrying, play parry sound and ripose
            if (parrying)
            {
                if (parryCoroutine != null)
                {
                    StopCoroutine(parryCoroutine);
                    parryCoroutine = null;
                }
                PlayerController.Instance.audioSource.PlayOneShot(PlayerController.Instance.parrySound);
                parrying = false;

                if (parryParticles != null) parryParticles.Play();

                impulseSource.GenerateImpulse();
                PlayerController.Instance.HitStopTime(0, 5, .5f);
                StartCoroutine(Slash()); // Riposte
                return; // Exit early if parrying
            }

            if (canStun)
            {
                hitCounter++;
                if (hitCounter >= 12)
                {
                    ResetAllAttacks();
                    StartCoroutine(Stunned());
                }
            }
            base.EnemyHit(_damageDone, _hitDirection, _hitForce);
            if (damageFlash != null) damageFlash.CallDamageFlash();
            if (!attacking && !stunned)
            {
                if (parryCoroutine != null) StopCoroutine(parryCoroutine);
                parryCoroutine = StartCoroutine(Parry());
            }

            #region Health to state

            float _healthRatio = health / maxHealth;
            if (_healthRatio > .75f)
            {
                ChangeState(EnemyStates.THK_Stage1);
                Debug.Log("THK Stage 1");
            }
            else if (_healthRatio <= .75f && _healthRatio > .5f)
            {
                ChangeState(EnemyStates.THK_Stage2);
                Debug.Log("THK Stage 2");
            }
            else if (_healthRatio <= .5f && _healthRatio > .25f)
            {
                ChangeState(EnemyStates.THK_Stage3);
                Debug.Log("THK Stage 3");
            }
            else if (_healthRatio <= .25f && health > 0f)
            {
                ChangeState(EnemyStates.THK_Stage4);
                Debug.Log("THK Stage 4");
            }
            else if (_healthRatio <= 0f && alive)
            {
                Death(0f);
                Debug.Log("THK Dead");
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -25);
            animator.SetTrigger("Die");
        }

        public void DestroyAfterDeath()
        {
            Destroy(gameObject);
            GameManager.Instance.BossKilled();
            SpawnBoss.Instance.isBossDead = true;
        }
        #endregion

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

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * attackRange);
        }
    }
}