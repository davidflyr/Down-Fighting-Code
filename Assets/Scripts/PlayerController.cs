using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start() variables
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    private PlayerCombat combat;

    // Animation state
    private enum State { idle, running, jumping, falling, attacking, hurt, death };
    [SerializeField] private State state = State.idle;

    // Inspector movement variables
    [SerializeField] private LayerMask ground;
    [SerializeField] private float maxspeed = 10f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private float hurtforce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 3f;

    // isGrounded check variables
    [SerializeField] private bool isGrounded;
    [SerializeField] private float yBound;
    [SerializeField] private float xBound;
    [SerializeField] private float groundedSideShell = 0.15f;
    [SerializeField] private float groundedCheckDepth = .01f;
    private Vector2 collCenter;

    // Special jump feature variables
    [SerializeField] private float jumpPressedRemember = 0;
    [SerializeField] private float jumpPressedRememberTime = 0.2f;
    [SerializeField] private float groundedRemember = 0;
    [SerializeField] private float groundedRememberTime = 0.2f;

    void Start()
    {
        transform.localScale = new Vector2(-1, 1);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        combat = GetComponent<PlayerCombat>();

        yBound = coll.bounds.extents.y;
        xBound = coll.bounds.extents.x;
    }

    void Update()
    {
        if (state != State.hurt)
            Movement();
        AnimationState();
        anim.SetInteger("state", (int)state);
    }

    private void GroundCheck()
    {
        collCenter = new Vector2(transform.position.x + coll.offset.x, transform.position.y + coll.offset.y);

        isGrounded = Physics2D.OverlapArea(new Vector2(collCenter.x - (xBound - groundedSideShell), collCenter.y - yBound),
            new Vector2(collCenter.x + (xBound - groundedSideShell), collCenter.y - (yBound + groundedCheckDepth)),
            ground);
    }

    private void Movement()
    {
        GroundCheck();

        bool isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName("LightBandit_Attack");

        float speedslope = (maxspeed - 20) / .9f;
        float speedintercept = (-0.1f * speedslope) + 20;
        float speedmultiplier;


        float HDirection = Input.GetAxis("Horizontal");

        jumpPressedRemember -= Time.deltaTime;
        groundedRemember -= Time.deltaTime;

        if (HDirection > 0)
        {
            speedmultiplier = (HDirection * speedslope) + speedintercept; //multiplier is positive
            rb.velocity = new Vector2(HDirection * speedmultiplier, rb.velocity.y);

            if (!isAttacking || combat.attackRemember > -0.1f)
                transform.localScale = new Vector2(-1, 1);
            
            if (!Input.GetButton("Horizontal") && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x / 5, rb.velocity.y);
        }

        if (HDirection < 0)
        {
            speedmultiplier = (HDirection * speedslope) - speedintercept; //multiplier is negative
            rb.velocity = new Vector2(HDirection * -speedmultiplier, rb.velocity.y);

            if (!isAttacking || combat.attackRemember > -0.1f)
                transform.localScale = new Vector2(1, 1);

            if (!Input.GetButton("Horizontal") && isGrounded)
                rb.velocity = new Vector2(rb.velocity.x / 5, rb.velocity.y);
        }

        if (Input.GetButtonDown("Jump"))
            jumpPressedRemember = jumpPressedRememberTime;

        if (isGrounded)
            groundedRemember = groundedRememberTime;

        if (jumpPressedRemember > 0 && groundedRemember > 0 && !isAttacking)
        {
            jumpPressedRemember = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
            state = State.jumping;
        }

        if (state == State.jumping)
        {
            groundedRemember = 0;

            // Jumping gravity stuff
            if (rb.velocity.y < 0)
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        if (isGrounded && isAttacking)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void AnimationState()
    {
        if (state == State.jumping)
        {
            if (rb.velocity.y < 0.1f)
            {
                state = State.falling;
            }
        }
        else if (state == State.falling)
        {
            if (isGrounded)
            {
                state = State.idle;
            }
        }
        else if (state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                state = State.idle;
            }
        }
        else if ((rb.velocity.y < 0) && !coll.IsTouchingLayers(ground))
        {
            state = State.falling;
        }
        else if ((Mathf.Abs(rb.velocity.x) > 2f) && coll.IsTouchingLayers(ground))
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireCube(new Vector2(transform.position.x + GetComponent<Collider2D>().offset.x,
            transform.position.y + GetComponent<Collider2D>().offset.y -
            GetComponent<Collider2D>().bounds.extents.y - .5f*groundedCheckDepth),
            new Vector2(2*GetComponent<Collider2D>().bounds.extents.x - 2*groundedSideShell, groundedCheckDepth));
    }
}
