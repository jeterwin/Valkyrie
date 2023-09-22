using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class MovementScript : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;

    [SerializeField] private AttackScript attackScript;

    [SerializeField] private SpriteRenderer SpriteRenderer;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Animator Animator;

    [SerializeField] private CapsuleCollider2D CapsuleCollider;


    [Header("Movement Variables")]
    [SerializeField] private float MovementSpeed;

    [Header("Jumping Variables")]
    [SerializeField] private float JumpSpeed = 1f;

    [Range(0f,50f)]
    [SerializeField] private float MaxYSpeed;

    [Range(0f,50f)]
    [SerializeField] private float MinYSpeed;

    [SerializeField] private float FallMultiplier = 2.5f;

    [SerializeField] private float LowJumpMultiplier = 2f;


    [Header("Dashing Variables")]
    [Range(1f,25f)]
    [SerializeField] private float DashDistance;

    [SerializeField] private float DashDuration;

    [SerializeField] private float DashCooldown;

    [SerializeField] private Color Color;

    private bool IsDashing = false;

    private bool CanDash = true;

    private Coroutine dashingCoroutine;


    [Header("Wallslide Variables")]
    [SerializeField] private float WallSlidingSpeed;

    [SerializeField] private LayerMask WallLayer;

    [SerializeField] private Transform WallCheck;

    [SerializeField] private ParticleSystem SlidingParticleSystem;

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
    [SerializeField] private Transform SpherePosition;
    [SerializeField] float SphereRadius;
    [SerializeField] private LayerMask GroundMask;
    private Vector2 dashDirection;

    public Rigidbody2D Rigidbody
    {
        get { return rb; }
        set { rb = value; }
    }
    public MovementState State { get; set; } = MovementState.Idle;

    private void Update()
    {
        ControlParticles();
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!CanDash) { return; }
            dashingCoroutine = StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {        
        UpdateAnimationState();
        ForceMaxSpeed();
        WallSlide();
        if(IsDashing)
        {
            rb.velocity = DashDistance * dashDirection;
            if(IsOnPad)
            {
                rb.AddForce(jumpPadStrength * Vector2.up, ForceMode2D.Impulse);
            }
            return;
        }
        rb.velocity = new Vector2(inputHandler.MovementAxis.x * MovementSpeed, rb.velocity.y);
        FallingLogic();
        if(IsOnPad) return;
        Jump();
    }
    private void ControlParticles()
    {
        if (!SlidingParticleSystem.isEmitting && isWallSliding)
            SlidingParticleSystem.Play();
        else if (!isWallSliding)
            SlidingParticleSystem.Stop();
    }

    private void Jump()
    {
        if(!inputHandler.PressedJump || !IsGrounded) { return; }

        rb.velocity = JumpSpeed * Vector2.up;
    }

    IEnumerator Dash()
    {
        IsDashing = true;
        CanDash = false;
        dashDirection = inputHandler.MovementAxis.normalized == Vector2.zero 
            ? new Vector2(transform.localScale.x, 0f) : inputHandler.MovementAxis.normalized;
        yield return new WaitForSeconds(DashDuration);
        IsDashing = false;
        yield return new WaitForSeconds(DashCooldown);
        CanDash = true;
        yield return null;
    }
    public bool IsGrounded
    {
        get { return Physics2D.OverlapCircle(SpherePosition.position, SphereRadius, GroundMask); }
    }
    bool IsWalled
    {
        get { return Physics2D.OverlapCircle(WallCheck.position, SphereRadius, WallLayer);}
    }

    private void WallSlide()
    {
        if(IsGrounded || !IsWalled || inputHandler.MovementAxis.x == 0)
        {
            isWallSliding = false;
            CapsuleCollider.size = new Vector2(1.311524f, CapsuleCollider.size.y);
            return;
        }

        isWallSliding = true;
        CapsuleCollider.size = new Vector2(0.93f, CapsuleCollider.size.y);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallSlidingSpeed, float.MaxValue));
    }

    private void FallingLogic()
    {
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Physics2D.gravity.y * FallMultiplier * Time.fixedDeltaTime * Vector2.up, ForceMode2D.Impulse);
        }
        else if (rb.velocity.y > 0 && !inputHandler.PressedJump && !IsOnPad)
        {
            rb.AddForce(Physics2D.gravity.y * LowJumpMultiplier * Time.fixedDeltaTime * Vector2.up, ForceMode2D.Impulse);
        }
    }

    private void ForceMaxSpeed()
    {
        if (rb.velocity.y > MaxYSpeed)
            rb.velocity = new Vector2(rb.velocity.x, MaxYSpeed);
        else if(rb.velocity.y < -MinYSpeed)
            rb.velocity = new Vector2(rb.velocity.x, -MinYSpeed);
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

        if(IsDashing)
            State = MovementState.Dashing;

        if(attackScript.isAttacking)
            State = MovementState.Attacking;

        if(State != MovementState.Jumping && State != MovementState.Falling && State != MovementState.Dashing)
            IsOnPad = false;

        Animator.SetInteger("State", (int)State);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(SpherePosition.position, SphereRadius);
        Gizmos.DrawWireSphere(WallCheck.position, SphereRadius);
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
