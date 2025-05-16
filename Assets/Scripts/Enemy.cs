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

    protected virtual void Start()
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
        health -= _damageDone;

        if (!isRecoiling) {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D _other) {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible) {
            if (PlayerController.Instance.pState.dashing) {
                PlayerController.Instance.pState.dashing = false;
            }
            Attack();
            PlayerController.Instance.HitStopTime(0, 5, 0.3f);
        }
    }

    protected virtual void Attack() {
        PlayerController.Instance.TakeDamage(damage);
    }
}
}

