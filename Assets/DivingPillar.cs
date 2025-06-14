using UnityEngine;

namespace Metroknight
{
    public class DivingPillar : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D _other)
        {
            if (_other.CompareTag("Player"))
            {
                _other.GetComponent<PlayerController>().TakeDamage(TheHollowKnight.Instance.damage);
            }
        }
    }
}
