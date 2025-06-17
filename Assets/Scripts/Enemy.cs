using UnityEngine;
using Cinemachine;

namespace Metroknight {
    public class Enemy : MonoBehaviour
    {
        [SerializeField] public float health;
        [SerializeField] public float maxHealth;

        [Header("Recoil")]
        [SerializeField] protected float recoilLength;
        [SerializeField] protected float recoilFactor;
        [SerializeField] protected bool isRecoiling = false;
        [Space(5)]

        [SerializeField] protected PlayerController player;
        [SerializeField] public float speed;
        [SerializeField] public float damage;
        [SerializeField] protected GameObject manaBlood;
        [SerializeField] protected float destroyTime;
        [SerializeField] protected AudioClip hitSound;
        protected DamageFlash damageFlash;

        protected float recoilTimer;
        public Rigidbody2D rb;
        protected SpriteRenderer sr;
        public Animator animator;
        protected AudioSource audioSource;
        protected CinemachineImpulseSource impulseSource;

        public string enemyName;

        protected enum EnemyStates
        {
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

            // Shade
            Shade_Idle,
            Shade_Chase,
            Shade_Stunned,
            Shade_Death,

            // THK
            THK_Stage1,
            THK_Stage2,
            THK_Stage3,
            THK_Stage4,
        };

        protected EnemyStates currentEnemyState;
        protected virtual EnemyStates GetCurrentEnemyState
        {
            get { return currentEnemyState; }
            set
            {
                if (currentEnemyState != value)
                {
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
            audioSource = GetComponent<AudioSource>();
            damageFlash = GetComponent<DamageFlash>();
            impulseSource = GetComponent<CinemachineImpulseSource>();
            // TODO change this to health = maxHealth
            health = maxHealth;
        }


        // Update is called once per frame
        protected virtual void Update()
        {
            if (GameManager.Instance.gameIsPaused) return;

            if (isRecoiling)
            {
                if (recoilTimer < recoilLength)
                {
                    recoilTimer += Time.deltaTime;
                }
                else
                {
                    isRecoiling = false;
                    recoilTimer = 0;
                }
            }
            else
            {
                UpdateEnemyStates();
            }
        }

        public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
        {
            health -= _damageDone;

            if (!isRecoiling && health > 0)
            {
                audioSource.PlayOneShot(hitSound);

                // Avoid NullReferenceException
                if (damageFlash != null) damageFlash.CallDamageFlash();
                if (impulseSource != null) CameraShakeManager.Instance.CameraShake(impulseSource);

                GameObject _manaBlood = Instantiate(manaBlood, transform.position, Quaternion.identity);
                Destroy(_manaBlood, 1f);
                rb.velocity = _hitForce * recoilFactor * _hitDirection;
                isRecoiling = true;
            }
        }

        protected virtual void OnCollisionStay2D(Collision2D _other)
        {
            if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
            {
                if (PlayerController.Instance.pState.dashing)
                {
                    PlayerController.Instance.pState.dashing = false;
                }
                Attack();
                if (PlayerController.Instance.pState.alive)
                {
                    PlayerController.Instance.HitStopTime(0, 5, 0.3f);
                }
            }
        }

        protected virtual void Death(float _destroyTime)
        {
            Destroy(gameObject, _destroyTime);
        }

        protected virtual void Attack()
        {
            PlayerController.Instance.TakeDamage(damage);
        }

        protected virtual void UpdateEnemyStates() { }

        protected virtual void ChangeCurrentAnimation() { }

        protected void ChangeState(EnemyStates _newState)
        {
            GetCurrentEnemyState = _newState;
        }
        
        public float GetHealth()
        {
            return health;
        }
    }
}

