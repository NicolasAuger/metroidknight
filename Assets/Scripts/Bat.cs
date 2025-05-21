using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
public class Bat : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    float timer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Bat_Idle);
    }
    
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Bat_Idle);
        }
    }

    protected override void UpdateEnemyStates()
        {
            float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

            switch (GetCurrentEnemyState)
            {
                case EnemyStates.Bat_Idle:
                    rb.velocity = new Vector2(0, 0);
                    if (_dist < chaseDistance)
                    {
                        ChangeState(EnemyStates.Bat_Chase);
                    }
                    break;

                case EnemyStates.Bat_Chase:
                    rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, speed * Time.deltaTime));
                    FlipBat();
                    if (_dist > chaseDistance)
                    {
                        ChangeState(EnemyStates.Bat_Idle);
                    }
                    break;

                case EnemyStates.Bat_Stunned:
                    timer += Time.deltaTime;
                    if (timer >= stunDuration)
                    {
                        ChangeState(EnemyStates.Bat_Idle);
                        timer = 0;
                    }
                    // Handle stunned state
                    break;

                case EnemyStates.Bat_Death:
                    Death(Random.Range(5, 10));
                    break;
            }

        }

    void FlipBat() {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
        if (health > 0)
        {
            ChangeState(EnemyStates.Bat_Stunned);
        } else {
            ChangeState(EnemyStates.Bat_Death);
        }
    }

    protected override void ChangeCurrentAnimation()
    {
      animator.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Bat_Idle);
      animator.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Bat_Chase);
      animator.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Bat_Stunned);

      if (GetCurrentEnemyState == EnemyStates.Bat_Death)
      {
        animator.SetTrigger("Death");
      }
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
