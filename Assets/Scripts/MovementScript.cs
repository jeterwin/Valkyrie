using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class MovementScript : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Animator animator;

    [SerializeField] private DeathScript deathScript;

    public Animator Animator
    {
        get { return animator; }
        set { animator = value; }
    }

    [SerializeField] private CapsuleCollider2D capsuleCollider;


    [Header("Movement Variables")]
    [SerializeField] private float movementSpeed;

    [Header("Jumping Variables")]
    [SerializeField] private float jumpSpeed = 1f;

    [Range(0f,50f)]
    [SerializeField] private float maxYSpeed;

    [Range(0f,50f)]
    [SerializeField] private float minYSpeed;

    [SerializeField] private float fallMultiplier = 2.5f;

    [SerializeField] private float lowJumpMultiplier = 2f;


    [Header("Dashing Variables")]
    [Range(1f,25f)]
    [SerializeField] private float dashDistance;

    [SerializeField] private float dashDuration;

    [SerializeField] private float dashCooldown;

    [SerializeField] private Color color;

    private bool isDashing = false;

    private bool canDash = true;

    private Coroutine dashingCoroutine;


    [Header("Wallslide Variables")]
    [SerializeField] private float wallSlidingSpeed;

    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private Transform wallCheck;

    [SerializeField] private ParticleSystem slidingParticleSystem;

    private bool isWallSliding = false;

    [Header("Jump Pad Variables")]
    private bool isOnPad = false;
    private float jumpPadStrength;

    public bool IsOnPad
    {
        get { return isOnPad; }
        set { isOnPad = value; }
    }
    public float JumpPadStrength
    {
        get { return jumpPadStrength; }
        set { jumpPadStrength = value; }
    }
    [Header("Ground Check")]
    [SerializeField] private Transform spherePosition;
    [SerializeField] private float sphereRadius;
    [SerializeField] private LayerMask groundMask;
    private Vector2 dashDirection;

    public Rigidbody2D Rigidbody
    {
        get { return rb; }
        set { rb = value; }
    }
    public MovementState State { get; set; } = MovementState.Idle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; }

        HandleDeath();
    }

    private void HandleDeath()
    {
        rb.bodyType = RigidbodyType2D.Static;
        animator.SetTrigger("OnDeath");
        deathScript.Animator.Play("DeathAnimation");
    }

    private void Update()
    {
        ControlParticles();
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!canDash) { return; }
            dashingCoroutine = StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {        
        UpdateAnimationState();
        ForceMaxSpeed();
        WallSlide();
        if(isDashing)
        {
            rb.velocity = dashDistance * dashDirection;
            if(IsOnPad)
            {
                rb.AddForce(jumpPadStrength * Vector2.up, ForceMode2D.Impulse);
            }
            return;
        }
        rb.velocity = new Vector2(inputHandler.MovementAxis.x * movementSpeed, rb.velocity.y);
        FallingLogic();
        if(IsOnPad) return;
        Jump();
    }
    private void ControlParticles()
    {
        if (!slidingParticleSystem.isEmitting && isWallSliding)
            slidingParticleSystem.Play();
        else if (!isWallSliding)
            slidingParticleSystem.Stop();
    }

    private void Jump()
    {
        if(!inputHandler.PressedJump || !IsGrounded) { return; }

        rb.velocity = jumpSpeed * Vector2.up;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        dashDirection = inputHandler.MovementAxis.normalized == Vector2.zero 
            ? new Vector2(transform.localScale.x, 0f) : inputHandler.MovementAxis.normalized;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
        yield return null;
    }
    public bool IsGrounded
    {
        get { return Physics2D.OverlapCircle(spherePosition.position, sphereRadius, groundMask); }
    }
    bool IsWalled
    {
        get { return Physics2D.OverlapCircle(wallCheck.position, sphereRadius, wallLayer);}
    }

    private void WallSlide()
    {
        if(IsGrounded || !IsWalled || inputHandler.MovementAxis.x == 0)
        {
            isWallSliding = false;
            capsuleCollider.size = new Vector2(1.311524f, capsuleCollider.size.y);
            return;
        }

        isWallSliding = true;
        capsuleCollider.size = new Vector2(0.93f, capsuleCollider.size.y);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
    }

    private void FallingLogic()
    {
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Physics2D.gravity.y * fallMultiplier * Time.fixedDeltaTime * Vector2.up, ForceMode2D.Impulse);
        }
        else if (rb.velocity.y > 0 && !inputHandler.PressedJump && !IsOnPad)
        {
            rb.AddForce(Physics2D.gravity.y * lowJumpMultiplier * Time.fixedDeltaTime * Vector2.up, ForceMode2D.Impulse);
        }
    }

    private void ForceMaxSpeed()
    {
        if (rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector2(rb.velocity.x, maxYSpeed);
        else if(rb.velocity.y < -minYSpeed)
            rb.velocity = new Vector2(rb.velocity.x, -minYSpeed);
    }

    private void UpdateAnimationState()
    {
        if(rb.velocity.x > 0.01f)
        {
            State = MovementState.Walking;
            transform.localScale = new Vector3(.8f, transform.localScale.y, transform.localScale.z);
        }
        else if(rb.velocity.x < -0.01f)
        {
            State = MovementState.Walking;
            transform.localScale = new Vector3(-.8f, transform.localScale.y, transform.localScale.z);
        }
        else
            State = MovementState.Idle;

        if(rb.velocity.y > 0.01f)
        {
            State = MovementState.Jumping;
        }
        else if(rb.velocity.y < -0.01f)
        {
            State = MovementState.Falling;
        }

        if(isWallSliding)
            State = MovementState.WallSliding;

        if(isDashing)
            State = MovementState.Dashing;

        if(State != MovementState.Jumping && State != MovementState.Falling && State != MovementState.Dashing)
            IsOnPad = false;

        animator.SetInteger("State", (int)State);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spherePosition.position, sphereRadius);
        Gizmos.DrawWireSphere(wallCheck.position, sphereRadius);
    }

    public enum MovementState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Attacking,
        WallSliding,
        Dashing
    }
}
