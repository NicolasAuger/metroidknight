using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class Boss_Lunge : StateMachineBehaviour
    {
        Rigidbody2D rb;
        private bool isFacingRight;
        private float originalGravityScale;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb = animator.GetComponentInParent<Rigidbody2D>();
            isFacingRight = TheHollowKnight.Instance.facingRight;
            originalGravityScale = rb.gravityScale;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb.gravityScale = 0f; // Disable gravity during lunge
            int _dir = isFacingRight ? 1 : -1; // Determine direction based on facing

            rb.linearVelocity = new Vector2(_dir * (TheHollowKnight.Instance.speed * 5), 0f);

            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= TheHollowKnight.Instance.attackRange &&
                // Check that the player is not above the boss but right in front of it
                // Go with this for now, might need to adjust later
                PlayerController.Instance.transform.position.y <= rb.position.y + 0.2f &&
                PlayerController.Instance.transform.position.y >= rb.position.y - 0.2f &&
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
            TheHollowKnight.Instance.lunging = false;
            TheHollowKnight.Instance.attacking = false;
            TheHollowKnight.Instance.damagedPlayer = false;
            rb.gravityScale = originalGravityScale; // Restore gravity after lunge
        }
    }
}
