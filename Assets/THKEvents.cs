using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight
{
    public class THKEvents : MonoBehaviour
    {

        // Used as an event in Boss_Slash animation
        void SlashDamagePlayer()
        {
            if (PlayerController.Instance.transform.position.x > transform.position.x ||
                PlayerController.Instance.transform.position.x < transform.position.x)
            {
                Hit(TheHollowKnight.Instance.sideAttackTransform, TheHollowKnight.Instance.sideAttackArea);
            }
            else if (PlayerController.Instance.transform.position.y > transform.position.y)
            {
                Hit(TheHollowKnight.Instance.upAttackTransform, TheHollowKnight.Instance.upAttackArea);
            }
            else if (PlayerController.Instance.transform.position.y < transform.position.y)
            {
                Hit(TheHollowKnight.Instance.downAttackTransform, TheHollowKnight.Instance.downAttackArea);
            }
        }
        void Hit(Transform _attackTransform, Vector2 _attackArea)
        {
            Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0f);
            for (int i = 0; i < _objectsToHit.Length; i++)
            {
                if (_objectsToHit[i].GetComponent<PlayerController>() != null)
                {
                    PlayerController.Instance.TakeDamage(TheHollowKnight.Instance.damage);
                }
            }
        }

        // Used as an event in Boss_Parry animation
        void Parrying()
        {
            TheHollowKnight.Instance.parrying = true;
        }

        // Used as an event in Boss_BendDown animation last frame
        void BendDownCheck()
        {
            if (TheHollowKnight.Instance.barrageAttack)
            {
                StartCoroutine(BarrageAttackTransition());
            }
            if (TheHollowKnight.Instance.outbreakAttack)
            {
                StartCoroutine(OutbreakAttackTransition());
            }
            if (TheHollowKnight.Instance.bounceAttack)
            {
                TheHollowKnight.Instance.animator.SetTrigger("Bounce1");
            }
        }

        // Used as an event in Boss_Cast animation frame1
        void BarrageOrOutbreak()
        {
            if (TheHollowKnight.Instance.barrageAttack)
            {
                TheHollowKnight.Instance.StartCoroutine(TheHollowKnight.Instance.Barrage());
            }
            if (TheHollowKnight.Instance.outbreakAttack)
            {
                TheHollowKnight.Instance.StartCoroutine(TheHollowKnight.Instance.Outbreak());
            }
        }

        IEnumerator BarrageAttackTransition()
        {
            yield return new WaitForSeconds(1f);
            TheHollowKnight.Instance.animator.SetBool("Cast", true);
        }

        IEnumerator OutbreakAttackTransition()
        {
            yield return new WaitForSeconds(1f);
            TheHollowKnight.Instance.animator.SetBool("Cast", true);
        }

        void DestroyAfterDeath()
        {
            TheHollowKnight.Instance.DestroyAfterDeath();
        }
    }
}