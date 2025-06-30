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
                Vector2 _adjustedTarget = GetSafeTargetPosition();

                Vector2 _newPos = Vector2.MoveTowards(
                    rb.position,
                    _adjustedTarget,
                    TheHollowKnight.Instance.speed * 2f * Time.fixedDeltaTime
                );
                rb.MovePosition(_newPos);

                float _distance = Vector2.Distance(rb.position, _adjustedTarget);
                if (_distance < 0.1f)
                {
                    TheHollowKnight.Instance.rb.constraints = RigidbodyConstraints2D.FreezePosition;
                }
            }
        }

        Vector2 GetSafeTargetPosition()
        {
            Vector2 originalTarget = TheHollowKnight.Instance.moveToPosition;
            float safeDistance = 2f;
            
            // Check left and right for walls
            bool wallOnLeft = Physics2D.Raycast(originalTarget, Vector2.left, safeDistance, LayerMask.GetMask("Ground"));
            bool wallOnRight = Physics2D.Raycast(originalTarget, Vector2.right, safeDistance, LayerMask.GetMask("Ground"));
            
            if (wallOnLeft)
            {
                originalTarget.x += safeDistance;
            }
            else if (wallOnRight)
            {
                originalTarget.x -= safeDistance;
            }
            
            return originalTarget;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger("BendDown");
        }
    }
}