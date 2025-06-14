using Cinemachine;
using UnityEngine;

namespace Metroknight
{
    public class Boss_Bounce2 : StateMachineBehaviour
    {
        Rigidbody2D rb;
        bool callOnce;
        CinemachineImpulseSource impulseSource;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb = animator.GetComponentInParent<Rigidbody2D>();
            impulseSource = animator.GetComponentInParent<CinemachineImpulseSource>();
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Vector2 _forceDirection = new Vector2(
                Mathf.Cos(Mathf.Deg2Rad * TheHollowKnight.Instance.rotationDirectionToTarget),
                Mathf.Sin(Mathf.Deg2Rad * TheHollowKnight.Instance.rotationDirectionToTarget)
            );

            rb.AddForce(_forceDirection * 2, ForceMode2D.Impulse);
            TheHollowKnight.Instance.divingCollider.SetActive(true);

            if (TheHollowKnight.Instance.Grounded())
            {
                TheHollowKnight.Instance.divingCollider.SetActive(false);

                if (!callOnce)
                {
                    TheHollowKnight.Instance.ResetAllAttacks();
                    if (impulseSource != null) CameraShakeManager.Instance.CameraShake(impulseSource);
                    Instantiate(TheHollowKnight.Instance.groundShakeSmoke, TheHollowKnight.Instance.groundCheckPoint.position, Quaternion.identity);
                    TheHollowKnight.Instance.CheckBounce();
                    callOnce = true;
                }
                animator.SetTrigger("Grounded");
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger("Bounce2");
            animator.ResetTrigger("Grounded");
            callOnce = false;
        }
    }
}
