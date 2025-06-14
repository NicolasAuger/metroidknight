using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class Boss_Lunge : StateMachineBehaviour
    {
        Rigidbody2D rb;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb = animator.GetComponentInParent<Rigidbody2D>();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb.gravityScale = 0f; // Disable gravity during lunge
            int _dir = TheHollowKnight.Instance.facingRight ? 1 : -1;
            rb.velocity = new Vector2(_dir * (TheHollowKnight.Instance.speed * 5), 0f);

            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= TheHollowKnight.Instance.attackRange &&
                !TheHollowKnight.Instance.damagedPlayer && !PlayerController.Instance.pState.invincible)
            {
                PlayerController.Instance.TakeDamage(TheHollowKnight.Instance.damage);
                if (PlayerController.Instance.pState.alive)
                {
                    PlayerController.Instance.HitStopTime(0, 5, .5f);
                }
                TheHollowKnight.Instance.damagedPlayer = true; // Prevent multiple hits in one lunge
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}
