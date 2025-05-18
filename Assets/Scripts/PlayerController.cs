using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Metroknight {
public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpBufferFrames;
    [SerializeField] private float coyoteTime;
    [SerializeField] private int maxAirJumps;
    private float jumpBufferCounter = 0;
    private float coyoteTimeCounter = 0;
    private int airJumpCounter = 0;
    [Space(5)]


    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    [Space(5)]

    [Header("Attack")]
    [SerializeField] float damage;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] Transform sideAttackTransform, upAttackTransform, downAttackTransform;
    [SerializeField] Vector2 sideAttackArea, upAttackArea, downAttackArea;
    [SerializeField] GameObject slashEffect;
    bool attacking;
    [SerializeField] private float timeBetweenAttack;
    private float timeSinceLastAttack;
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Health")]
    public int health;
    public int maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    float healTimer;
    [SerializeField] float timeToHeal;
    // private bool canFlash;
    [Space(5)]

    [Header("Mana")]
    [SerializeField] Image manaStorage;
    [SerializeField] private float mana;
    [SerializeField] private float manaDrainSpeed;
    [SerializeField] private float manaGain;
    [Space(5)]

    [Header("Spell Casting")]
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast = 0.5f;
    [SerializeField] float spellDamage; // Up & down only
    [SerializeField] float downSpellForce; // down only
    float timeSinceLastCast;
    float castOrHealTimer;

    // SpellCast objects
    [SerializeField] GameObject sideSpellFireball;
    [SerializeField] GameObject upSpellExplosion;
    [SerializeField] GameObject downSpellFireball;

    [Space(5)]

    [HideInInspector] public PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator animator;
    private float gravity;
    private float xAxis, yAxis;
    private bool canDash = true;
    private bool dashed;
    private SpriteRenderer sr;

    public int Health {
        get { return health; }
        set {
            if (health != value) {
                health = Mathf.Clamp(value, 0, maxHealth);
                if (onHealthChangedCallback != null) {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    public float Mana {
        get { return mana; }
        set {
            if (mana != value) {
                mana = Mathf.Clamp(value, 0, 1);
                manaStorage.fillAmount = mana;
            }
        }
    }

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        Health = maxHealth;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        gravity = rb.gravityScale;

        Mana = mana;
        manaStorage.fillAmount = Mana;
    }

    // Update is called once per frame
    void Update()
    {
        RestoreTimeScale();
        if(pState.cutscene) return;

        GetInputs();
        UpdateJumpVariables();
        FlashWhenInvincible();

        if (pState.dashing) return;
        Move();
        Heal();
        CastSpells();

        if (pState.healing) return;
        Flip();
        Jump();
        StartDash();
        Attack();
    }

    private void OnTriggerEnter2D(Collider2D _other) // For up & down spells
    {
      if (_other.GetComponent<Enemy>() != null && pState.casting) {
        _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
      }
    }

    private void FixedUpdate() {
        if (pState.cutscene) return;
        if (pState.dashing) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attacking = Input.GetButtonDown("Attack");

        if (Input.GetButton("Cast/Heal")) {
            castOrHealTimer += Time.deltaTime;
        } else {
            castOrHealTimer = 0;
        }
    }

    private void Move() {
        if (pState.healing) rb.velocity = new Vector2(0, 0);
        rb.velocity = new Vector2(xAxis * walkSpeed, rb.velocity.y);
        animator.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    public bool Grounded() {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundLayer) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckX, groundLayer) ||
            Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckX, groundLayer)) {
            return true;
        } else {
            return false;
        }
    }

    void Flip() {
        // Flip the player using the Y euler rotation angle based on the xAxis
        if (xAxis > 0) {
            // Right
            transform.eulerAngles = new Vector3(0, 0, 0);
            pState.lookingRight = true;
        } else if (xAxis < 0) {
            // Left
            transform.eulerAngles = new Vector3(0, 180, 0);
            pState.lookingRight = false;
        }
    }

    public void Jump() {
        // Jump only if the player is grounded
        if (!pState.jumping) {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0) {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                pState.jumping = true;
            } else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump")) {
                pState.jumping = true;
                airJumpCounter++;
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            }
        }

        // Variable jump height
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0) {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }

        animator.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpVariables() {
        if (Grounded()) {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        } else {
            // Time.deltaTime is the time between frames
            // So we decrease coyoteTimeCounter by 1 every second
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferFrames;
        } else {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10;
        }
    }

    void StartDash() {
        if (Input.GetButtonDown("Dash") && canDash && !dashed) {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded()) {
            dashed = false;
        }
    }

    IEnumerator Dash() {
        canDash = false;
        pState.dashing = true;
        animator.SetTrigger("Dashing");
        rb.gravityScale = 0;
        int _dir = pState.lookingRight ? 1 : -1;
        // if (pState.lookingRight) {
        //     rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        // } else {
        //     rb.velocity = new Vector2(-transform.localScale.x * dashSpeed, 0);
        // }
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Attack() {
        timeSinceLastAttack += Time.deltaTime;
        if (attacking && timeSinceLastAttack >= timeBetweenAttack) {
            timeSinceLastAttack = 0;
            animator.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && Grounded()) {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;
                Hit(sideAttackTransform, sideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
                Instantiate(slashEffect, sideAttackTransform);
                // slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
            } else if (yAxis > 0) {
                Hit(upAttackTransform, upAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 90, upAttackTransform);
            } else if (yAxis < 0 && !Grounded()) {
                Hit(downAttackTransform, downAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, downAttackTransform);
            }
        }

    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength) {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        // Save the enemies that have been hit
        List<Enemy> hitEnemies = new List<Enemy>();

        if (objectsToHit.Length > 0) {
            _recoilBool = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++) {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();

            // This is to prevent the player from hitting the same enemy multiple times in one attack
            if (e && !hitEnemies.Contains(e)) {
                e.EnemyHit(damage, _recoilDir, _recoilStrength);
                hitEnemies.Add(e);

                if (objectsToHit[i].CompareTag("Enemy")) {
                    Mana += manaGain;
                }
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform) {
       _slashEffect = Instantiate(_slashEffect, _attackTransform);
       _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
       _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil() {
        if (pState.recoilingX) {
            if (pState.lookingRight) {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            } else {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY) {
            rb.gravityScale = 0;
            if (yAxis < 0) {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            } else if (yAxis > 0) {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }

            // Might fix the air jump bug not being reset while y recoiling
            airJumpCounter = 0;
            pState.jumping = false;
        } else {
            rb.gravityScale = gravity;
        }

        // Stop recoiling
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps) {
            stepsXRecoiled++;
        } else {
            StopRecoilX();
        }
        if (pState.recoilingY && stepsYRecoiled < recoilYSteps) {
            stepsYRecoiled++;
        } else {
            StopRecoilY();
        }

        if (Grounded()) {
            StopRecoilY();
        }
    }

    void StopRecoilX() {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY() {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage) {
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }

    IEnumerator StopTakingDamage() {
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        pState.invincible = true;
        animator.SetTrigger("Damaged");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    // Trigger sprite renderer invisibility instead of color
    // IEnumerator Flash() {
    //     sr.enabled = !sr.enabled;
    //     canFlash = false;
    //     yield return new WaitForSeconds(0.2f);
    //     canFlash = true;
    // }

    void RestoreTimeScale() {
        if (restoreTime) {
            if (Time.timeScale < 1) {
                // Use unscaledDeltaTime to restore time scale cuz the game is paused (time scale = 0)
                Time.timeScale += restoreTimeSpeed * Time.unscaledDeltaTime;
            } else {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay) {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if (_delay > 0) {
            StopCoroutine(RestoreTime(_delay));
            StartCoroutine(RestoreTime(_delay));
        } else {
            restoreTime = true;
        }
    }

    IEnumerator RestoreTime(float _delay) {
        // Use WaitForSecondsRealtime to use real time instead of game time since the game is paused
        yield return new WaitForSecondsRealtime(_delay);
        restoreTime = true;
    }

    void FlashWhenInvincible() {
        // Trigger sprite renderer invisibility instead of color
        // if (pState.invincible) {
        //     if (Time.timeScale > 0.2 && canFlash) {
        //         StartCoroutine(Flash());
        //     }
        // } else {
        //     sr.enabled = true;
        // }
        sr.material.color = (pState.invincible && !pState.cutscene) ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1f)) : Color.white;
    }

    void Heal() {
        if (Input.GetButton("Cast/Heal") && castOrHealTimer > 0.05f && Health < maxHealth && Mana > 0 && Grounded() && !pState.dashing) {
            pState.healing = true;
            animator.SetBool("Healing", true);

            // Healing
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal) {
                Health++;
                healTimer = 0;
            }

            // Drain mana
            Mana -= Time.deltaTime * manaDrainSpeed;;
        } else {
            pState.healing = false;
            healTimer = 0;
            animator.SetBool("Healing", false);
        }
    }

    void CastSpells() {
        if (Input.GetButtonUp("Cast/Heal") && castOrHealTimer <= 0.05f && Mana >= manaSpellCost && timeSinceLastCast >= timeBetweenCast) {
            pState.casting = true;
            timeSinceLastCast = 0;
            StartCoroutine(CastCoroutine());
        } else {
            timeSinceLastCast += Time.deltaTime;
        }

        // Disable down spell fireball if the player reaches the ground
        if (Grounded()) {
            downSpellFireball.SetActive(false);
        }

        // If down spell fireball is active, force player to go down till the ground
        if (downSpellFireball.activeInHierarchy) {
            rb.velocity += downSpellForce * Vector2.down;
        }
    }

    IEnumerator CastCoroutine() {
        animator.SetBool("Casting", true);
        yield return new WaitForSeconds(0.15f);

        // Side cast
        if (yAxis == 0 || (yAxis < 0 && Grounded())) {
            GameObject _fireball = Instantiate(sideSpellFireball, sideAttackTransform.position, Quaternion.identity);

            // Flip fireball
            if (pState.lookingRight) {
                _fireball.transform.eulerAngles = Vector3.zero;
            } else {
                _fireball.transform.eulerAngles = new Vector2(_fireball.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;
        }

        // Up cast
        else if (yAxis > 0) {
            Instantiate(upSpellExplosion, transform);
            rb.velocity = Vector2.zero;
        }

        // Down cast
        else if (yAxis < 0 && !Grounded()) {
            downSpellFireball.SetActive(true);
        }

        Mana -= manaSpellCost;
        yield return new WaitForSeconds(0.35f);
        animator.SetBool("Casting", false);
        pState.casting = false;
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay) {
        pState.invincible = true;

        // If exit direction is upwards
        if (_exitDir.y > 0) {
            rb.velocity = jumpForce * _exitDir;
        }

        // If exit direction requires horitontal movement
        if (_exitDir.x != 0) {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Move();
        }
        Flip();
        yield return new WaitForSeconds(_delay);
        pState.invincible = false;
        pState.cutscene = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTransform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTransform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTransform.position, downAttackArea);
    }

  }
}