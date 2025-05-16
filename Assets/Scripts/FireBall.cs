using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Metroknight {
public class FireBall : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float hitForce;
    [SerializeField] float speed;
    [SerializeField] float lifeTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        transform.position += transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Enemy")) {
            // Negative hit force to push the enemy in the correct direction
            _other.GetComponent<Enemy>().EnemyHit(damage, (_other.transform.position - transform.position).normalized, -hitForce);
        }
    }
}
}
