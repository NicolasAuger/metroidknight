using UnityEngine;

namespace Metroknight
{
    public class Boss_Idle : StateMachineBehaviour
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
            // Since it's the idle state
            rb.velocity = Vector2.zero;
            RunToPlayer(animator);

            if (TheHollowKnight.Instance.attackCountdown <= 0)
            {
                TheHollowKnight.Instance.AttackHandler();
                TheHollowKnight.Instance.attackCountdown = Random.Range(TheHollowKnight.Instance.attackTimer - 1, TheHollowKnight.Instance.attackTimer + 1); ;
            }

            if (!TheHollowKnight.Instance.Grounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, -25f); // if not grounded, fall down
            }
        }

        void RunToPlayer(Animator animator)
        {
            if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) >= TheHollowKnight.Instance.attackRange)
            {
                animator.SetBool("Run", true);
            }
            else
            {
                return;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}
