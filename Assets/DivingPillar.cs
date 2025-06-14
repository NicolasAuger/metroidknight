using UnityEngine;

namespace Metroknight
{
    public class DivingPillar : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
            {
                _other.GetComponent<PlayerController>().TakeDamage(TheHollowKnight.Instance.damage);
                if (PlayerController.Instance.pState.alive)
                {
                    PlayerController.Instance.HitStopTime(0, 5, .5f);
                }
            }
        }
    }
}
