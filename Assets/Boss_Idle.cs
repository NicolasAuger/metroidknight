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
            if (TheHollowKnight.Instance.attacking || TheHollowKnight.Instance.parrying) return;

            // Since it's the idle state
            rb.linearVelocity = Vector2.zero;
            RunToPlayer(animator);

            if (TheHollowKnight.Instance.attackCountdown <= 0)
            {
                TheHollowKnight.Instance.AttackHandler();
                TheHollowKnight.Instance.attackCountdown = Random.Range(TheHollowKnight.Instance.attackTimer - 1, TheHollowKnight.Instance.attackTimer + 1); ;
            }

            if (!TheHollowKnight.Instance.Grounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -25f); // if not grounded, fall down
            }
        }

        void RunToPlayer(Animator animator)
        {
            float _distance = Vector2.Distance(PlayerController.Instance.transform.position, rb.position);
            // Run only if the distance between boss & player is greater than range + buffer zone
            if (_distance >= TheHollowKnight.Instance.attackRange + TheHollowKnight.Instance.bufferZone)
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
