using System.Collections;
using System.Collections.Generic;
using Metroknight;
using UnityEngine;

namespace Metroknight
{
    public class Shade : Enemy
    {
        [SerializeField] private float chaseDistance;
        [SerializeField] private float stunDuration;
        float timer;

        public static Shade Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            // DontDestroyOnLoad(gameObject);
            SaveData.Instance.SaveShade();
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            ChangeState(EnemyStates.Shade_Idle);
        }
        
        protected override void Update()
        {
            base.Update();
            if (!PlayerController.Instance.pState.alive)
            {
                ChangeState(EnemyStates.Shade_Idle);
            }
        }

        protected override void UpdateEnemyStates()
        {
            float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Shade_Idle:
                    rb.velocity = new Vector2(0, 0);
                    if (_dist < chaseDistance)
                    {
                        ChangeState(EnemyStates.Shade_Chase);
                    }
                    break;

                case EnemyStates.Shade_Chase:
                    rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, speed * Time.deltaTime));
                    FlipShade();
                    if (_dist > chaseDistance)
                    {
                        ChangeState(EnemyStates.Shade_Idle);
                    }
                    break;

                case EnemyStates.Shade_Stunned:
                    timer += Time.deltaTime;
                    if (timer >= stunDuration)
                    {
                        ChangeState(EnemyStates.Shade_Idle);
                        timer = 0;
                    }
                    // Handle stunned state
                    break;

                case EnemyStates.Shade_Death:
                    Death(Random.Range(5, 10));
                    break;
            }

        }

        void FlipShade() {
            sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
        }

        public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
        {
            base.EnemyHit(_damageDone, _hitDirection, _hitForce);
            if (health > 0)
            {
                ChangeState(EnemyStates.Shade_Stunned);
            } else {
                ChangeState(EnemyStates.Shade_Death);
            }
        }

        protected override void ChangeCurrentAnimation()
        {
            // animator.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Shade_Idle);
            // animator.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Shade_Chase);
            // animator.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Shade_Stunned);
            if (GetCurrentEnemyState == EnemyStates.Shade_Idle)
            {
                animator.Play("Player_Idle");
            }

            animator.SetBool("Walking", GetCurrentEnemyState == EnemyStates.Shade_Chase);

            if (GetCurrentEnemyState == EnemyStates.Shade_Death)
            {
                PlayerController.Instance.RestoreMana();
                SaveData.Instance.SavePlayer();
                animator.SetTrigger("Death");
                Destroy(gameObject, 0.9f);
            }
        }

        protected override void Attack()
        {
            animator.SetTrigger("Attacking");
            PlayerController.Instance.TakeDamage(damage);
        }

        protected override void Death(float _destroyTime)
        {
            rb.gravityScale = 12;
            base.Death(_destroyTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
