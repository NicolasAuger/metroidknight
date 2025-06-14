using UnityEngine;

namespace Metroknight
{
    public class Boss_BendDown : StateMachineBehaviour
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
            OutbreakAttack();
        }

        void OutbreakAttack()
        {
            if (TheHollowKnight.Instance.outbreakAttack)
            {
                Vector2 _newPos = Vector2.MoveTowards(
                    rb.position,
                    TheHollowKnight.Instance.moveToPosition,
                    TheHollowKnight.Instance.speed * 1.5f * Time.fixedDeltaTime
                );
                rb.MovePosition(_newPos);
                float _distance = Vector2.Distance(rb.position, _newPos);
                if (_distance < 0.1f)
                {
                    TheHollowKnight.Instance.rb.constraints = RigidbodyConstraints2D.FreezePosition;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger("BendDown");
        }
    }
}