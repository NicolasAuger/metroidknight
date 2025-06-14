using UnityEngine;
using Cinemachine;

namespace Metroknight
{
    public class Boss_Dive : StateMachineBehaviour
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
            TheHollowKnight.Instance.divingCollider.SetActive(true);
            if (TheHollowKnight.Instance.Grounded())
            {
                TheHollowKnight.Instance.divingCollider.SetActive(false);
                if (!callOnce)
                {
                    TheHollowKnight.Instance.DivingPillars();
                    if (impulseSource != null) CameraShakeManager.Instance.CameraShake(impulseSource);
                    Instantiate(TheHollowKnight.Instance.groundShakeSmoke, TheHollowKnight.Instance.groundCheckPoint.position, Quaternion.identity);
                    animator.SetBool("Dive", false);
                    TheHollowKnight.Instance.ResetAllAttacks();
                    callOnce = true;
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            callOnce = false;
        }
    }
}
