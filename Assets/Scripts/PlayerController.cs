using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Variables for moving
    public float maxSpeed;

    Rigidbody2D rb;
    Animator playerAnim;
    public PlayerHealth health;
    bool facingRight;

    // Vars for PlasmaBolts
    public Transform hand;
    public GameObject projectile;
    public GameObject arrowFire;
    public float fireRate = 1f;
    float nextFire = 0;

    int equiped = 0;

    // Attacking
    public Collider2D[] attackHitboxes;

    // Jumping stuffs
    public bool grounded = false;
    float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float jumpHeight;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private enum State { idle, walk, melee };
    private State playerState = State.idle;

    // Use this for initialization
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();

        facingRight = true;
    }

    void FixedUpdate() {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        float move = Input.GetAxisRaw("Horizontal");

        // Set playerState
        if (playerState != State.melee) {
            if (move == 0f) {
                playerState = State.idle;
            } else {
                playerState = State.walk;
            }
        }

        // Apply horizontal movement
        if (playerState == State.walk) {
            rb.AddForce(new Vector2(move * maxSpeed, 0));
        }

        // Flip player if needed
        if (move > 0 && !facingRight) {
            flip();
        } else if (move < 0 && facingRight) {
            flip();
        }


        // Set playAnim triggers
        playerAnim.SetBool("isWalking", playerState == State.walk);
        playerAnim.SetBool("isGrounded", grounded);
        playerAnim.SetFloat("verticalSpeed", rb.velocity.y);
    }

    void flip() {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Update() {
        // Jumping
        if (grounded && Input.GetButtonDown("Jump")) {
            grounded = false;
            rb.AddForce(new Vector2(0, jumpHeight));
        }

        // Smart Jump
        if (rb.velocity.y < 0) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Melee attack
        if (Input.GetButtonDown("Melee") && grounded && playerState != State.melee) {
            attack(attackHitboxes[0]);
            playerAnim.SetTrigger("melee");
        }

        // Switching Items
        if (Input.GetButtonDown("Fire3")) {
            if (equiped < 2) {
                equiped++;
            } else {
                equiped = 0;
            }
        }

        // Plasma Bolt
        if (Input.GetButtonDown("Fire1")) {
            firePlasma();
        }

        // Animation Updates
        playerAnim.SetBool("isGrounded", grounded);
    }

    // Melee hit
    void attack(Collider2D hitbox) {
        StartCoroutine(attackForTime(hitbox));
    }

    public IEnumerator attackForTime(Collider2D hitbox) {
        playerState = State.melee;
        hitbox.enabled = true;
        yield return new WaitForSeconds(.5f);
        hitbox.enabled = false;
        playerState = State.idle;
    }

    void firePlasma() {
        if (Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            if (equiped == 0) {
                if (facingRight) {
                    Instantiate(projectile, hand.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                } else if (!facingRight) {
                    Instantiate(projectile, hand.position, Quaternion.Euler(new Vector3(0, 0, 180)));
                }
            } else if (equiped == 1) {
                if (facingRight) {
                    Instantiate(arrowFire, hand.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                } else if (!facingRight) {
                    Instantiate(arrowFire, hand.position, Quaternion.Euler(new Vector3(0, 0, 180)));
                }
            } else {
                if (facingRight) {
                    Instantiate(projectile, hand.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                } else if (!facingRight) {
                    Instantiate(projectile, hand.position, Quaternion.Euler(new Vector3(0, 0, 180)));
                }
            }
        }
    }
}
