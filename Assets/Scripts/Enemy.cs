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
    [SerializeField] protected GameObject manaBlood;
    [SerializeField] protected float destroyTime;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator animator;


    protected enum EnemyStates {
        // Crawler
        Crawler_Idle,
        Crawler_Flip,

        // Bat
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,

        // Charger
        Charger_Idle,
        Charger_Surprised,
        Charger_Charge,

     };
    protected EnemyStates currentEnemyState;
    protected virtual EnemyStates GetCurrentEnemyState {
        get { return currentEnemyState; }
        set {
            if (currentEnemyState != value) {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = PlayerController.Instance;
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (isRecoiling) {
            if (recoilTimer < recoilLength) {
                recoilTimer += Time.deltaTime;
            } else {
                isRecoiling = false;
                recoilTimer = 0;
            }
        } else {
            UpdateEnemyStates();
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce) {
        health -= _damageDone;

        if (!isRecoiling && health > 0) {
            GameObject _manaBlood = Instantiate(manaBlood, transform.position, Quaternion.identity);
            Destroy(_manaBlood, 1f);
            rb.velocity = _hitForce * recoilFactor * _hitDirection;
            isRecoiling = true;
        }
    }

    protected virtual void OnCollisionStay2D(Collision2D _other) {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0) {
            if (PlayerController.Instance.pState.dashing) {
                PlayerController.Instance.pState.dashing = false;
            }
            Attack();
            PlayerController.Instance.HitStopTime(0, 5, 0.3f);
        }
    }

    protected virtual void Death(float _destroyTime) {
        Destroy(gameObject, _destroyTime);
    }

    protected virtual void Attack() {
        PlayerController.Instance.TakeDamage(damage);
    }

    protected virtual void UpdateEnemyStates() {}

    protected virtual void ChangeCurrentAnimation() {

    }

    protected void ChangeState(EnemyStates _newState) {
        GetCurrentEnemyState = _newState;
    }
}
}

