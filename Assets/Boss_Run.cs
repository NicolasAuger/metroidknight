using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class Boss_Run : StateMachineBehaviour
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

            TargetPlayerPosition(animator);

            if (TheHollowKnight.Instance.attackCountdown <= 0)
            {
                TheHollowKnight.Instance.AttackHandler();
                TheHollowKnight.Instance.attackCountdown = Random.Range(TheHollowKnight.Instance.attackTimer - 1, TheHollowKnight.Instance.attackTimer + 1); ;
            }
        }

        void TargetPlayerPosition(Animator animator)
        {
            if (TheHollowKnight.Instance.Grounded())
            {
                // TheHollowKnight.Instance.Flip();
                Vector2 _target = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y);
                Vector2 _newPos = Vector2.MoveTowards(rb.position, _target, TheHollowKnight.Instance.runSpeed * Time.fixedDeltaTime);
                rb.MovePosition(_newPos);
            }
            else
            {
                // If the boss is not grounded, make it fall to ground
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -25f);
            }

            float _distance = Vector2.Distance(PlayerController.Instance.transform.position, rb.position);

            // Stop running if the distance between boss & player is less than range + buffer zone
            if (_distance <= TheHollowKnight.Instance.attackRange - TheHollowKnight.Instance.bufferZone)
            {
                animator.SetBool("Run", false);
            }
            else
            {
                return;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("Run", false);
        }
    }
}