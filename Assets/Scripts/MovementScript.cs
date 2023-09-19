using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class MovementScript : MonoBehaviour
{
    private InputActions input;
    [SerializeField] SpriteRenderer SpriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator Animator;

    [Header("Movement Variables")]
    [SerializeField] float MovementSpeed;
    [Range(0f,5f)]
    [SerializeField] float MaxYSpeed;

    [Header("Jumping Variables")]
    [Range(1f,10f)]
    [SerializeField] float JumpSpeed = 1f;
    [SerializeField] float FallMultiplier = 2.5f;
    [SerializeField] float LowJumpMultiplier = 2f;

    [Header("Dashing Variables")]
    [Range(1f,25f)]
    [SerializeField] float DashDistance;
    [SerializeField] float DashDuration;
    [SerializeField] float DashCooldown;
    [SerializeField] Color Color;
    [SerializeField] private bool IsDashing = false;
    [SerializeField] private bool CanDash = true;
    Coroutine DashingCoroutine;

    [Header("Wall Slide Variables")]
    private bool IsWallSliding = false;
    [SerializeField] private float WallSlidingSpeed;
    [SerializeField] private LayerMask WallLayer;
    [SerializeField] private Transform WallCheck;

    [Header("Ground Check")]
    [SerializeField] Transform SpherePosition;
    [SerializeField] float SphereRadius;
    [SerializeField] LayerMask GroundMask;
    Vector2 DashDirection;

    MovementState State = MovementState.Idle;
    private bool IsHoldingJump = false;


    Vector2 MovementAxis;
    private void Awake()
    {
        input = new InputActions();
        input.ActionMap.Movement.started += OnMove;
        input.ActionMap.Movement.canceled += OnMove;
        input.ActionMap.Movement.performed += OnMove;
    }
    private void OnEnable()
    {
        input.ActionMap.Enable();
    }
    private void OnDisable()
    {
        input.ActionMap.Disable();
    }
    private void Update()
    {
        IsHoldingJump = Input.GetKey(KeyCode.Space);
        if(IsGrounded && DashingCoroutine != null)
        {
            StopCoroutine(DashingCoroutine);
            CanDash = true;
        }
        if(Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            Jump();
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(!CanDash) { return; }
            DashingCoroutine = StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {        
        UpdateAnimationState();
        ForceMaxSpeed();
        WallSlide();
        if(IsDashing)
        {
            rb.velocity = DashDistance * DashDirection;
            return;
        }
        rb.velocity = new Vector2(MovementAxis.x * MovementSpeed, rb.velocity.y);
        FallingLogic();
    }
    IEnumerator Dash()
    {
        IsDashing = true;
        CanDash = false;
        DashDirection = MovementAxis.normalized;
        yield return new WaitForSeconds(DashDuration);
        IsDashing = false;
        yield return new WaitForSeconds(DashCooldown);
        CanDash = true;
        yield return null;
    }
    bool IsGrounded
    {
        get { return Physics2D.OverlapCircle(SpherePosition.position, SphereRadius, GroundMask); }
    }
    bool IsWalled
    {
        get { return Physics2D.OverlapCircle(WallCheck.position, SphereRadius, WallLayer);}
    }
    private void Jump()
    {
        rb.velocity = JumpSpeed * Vector2.up;
    }

    private void WallSlide()
    {
        if(!IsGrounded && IsWalled && MovementAxis.x != 0)
        {
            IsWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallSlidingSpeed, float.MaxValue));
        }
        else
        {
            IsWallSliding = false;
        }
    }

    private void FallingLogic()
    {
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Physics2D.gravity.y * FallMultiplier * Time.fixedDeltaTime * Vector2.up, ForceMode2D.Impulse);
        }
        else if (rb.velocity.y > 0 && !IsHoldingJump)
        {
            rb.AddForce(Physics2D.gravity.y * LowJumpMultiplier * Time.fixedDeltaTime * Vector2.up, ForceMode2D.Impulse);
        }
    }

    private void ForceMaxSpeed()
    {
        if (rb.velocity.y > MaxYSpeed)
            rb.velocity = new Vector2(rb.velocity.x, MaxYSpeed);
        else if(rb.velocity.y < -MaxYSpeed)
            rb.velocity = new Vector2(rb.velocity.x, -MaxYSpeed);
    }

    private void UpdateAnimationState()
    {
        if(rb.velocity.x > 0.01f)
        {
            State = MovementState.Walking;
            transform.localScale = new Vector3(1f, transform.localScale.y, transform.localScale.z);
        }
        else if(rb.velocity.x < -0.01f)
        {
            State = MovementState.Walking;
            transform.localScale = new Vector3(-1f, transform.localScale.y, transform.localScale.z);
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

        if(IsWallSliding)
            State = MovementState.WallSliding;

        if(IsDashing)
            State = MovementState.Dashing;

        Animator.SetInteger("State", (int)State);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MovementAxis = context.ReadValue<Vector2>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(SpherePosition.position, SphereRadius);
        Gizmos.DrawWireSphere(WallCheck.position, SphereRadius);
    }

    enum MovementState
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
