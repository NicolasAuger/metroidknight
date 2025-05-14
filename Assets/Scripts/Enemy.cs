using UnityEngine;

namespace Metroknight {
public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;

    [Header("Recoil")]
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    [Space(5)]

    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;
    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if (isRecoiling) {
            if (recoilTimer < recoilLength) {
                recoilTimer += Time.deltaTime;
            } else {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce) {
        print("damage done: " + _damageDone);
        health -= _damageDone;

        if (!isRecoiling) {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D _other) {
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.invincible) {
            Attack();
            PlayerController.Instance.HitStopTime(0, 5, 0.5f);
        }
    }

    protected virtual void Attack() {
        PlayerController.Instance.TakeDamage(damage);
    }
}
}

