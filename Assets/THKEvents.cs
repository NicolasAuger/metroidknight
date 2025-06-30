using System.Collections;
using UnityEngine;

namespace Metroknight
{
    public class THKEvents : MonoBehaviour
    {

        // Used as an event in Boss_Slash animation
        void SlashDamagePlayer()
        {
            Vector2 _directionToPlayer = PlayerController.Instance.transform.position - transform.position;
            TheHollowKnight.Instance.SlashAngle();
            
            // Prioritize vertical attacks based on the player's position relative to the boss
            // if (_directionToPlayer.y > verticalThreshold)
            // {
            //     Hit(TheHollowKnight.Instance.upAttackTransform, TheHollowKnight.Instance.upAttackArea);
            // }

            // Will probably never happend, but just in case
            // else if (_directionToPlayer.y < -verticalThreshold)
            // {
            //     Hit(TheHollowKnight.Instance.downAttackTransform, TheHollowKnight.Instance.downAttackArea);
            // }
            // else
            // {
                // Side attack if the Y difference is not significant
                Hit(TheHollowKnight.Instance.sideAttackTransform, TheHollowKnight.Instance.sideAttackArea);
            // }
        }

        void Hit(Transform _attackTransform, Vector2 _attackArea)
        {
            Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0f);
            for (int i = 0; i < _objectsToHit.Length; i++)
            {
                if (_objectsToHit[i].GetComponent<PlayerController>() != null && !PlayerController.Instance.pState.invincible)
                {
                    _objectsToHit[i].GetComponent<PlayerController>().TakeDamage(TheHollowKnight.Instance.damage);

                    if (PlayerController.Instance.pState.alive)
                    {
                        PlayerController.Instance.HitStopTime(0, 5, .5f);
                    }
                }
            }
        }

        // Used as an event in Boss_Parry animation
        // Parrying property is now handled in main HollowKnight script
        void Parrying()
        {
            // TheHollowKnight.Instance.parrying = true;
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
            GameManager.Instance.THKDefeated = true;
            SaveData.Instance.SaveBosses();
            SaveData.Instance.SavePlayer();
        }
    }
}