using System;
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
    [Range(0,25f)]
    [SerializeField] float MaxSpeed;

    [Header("Jumping Variables")]
    [Range(1f,10f)]
    [SerializeField] float JumpSpeed = 1f;
    [SerializeField] float FallMultiplier = 2.5f;
    [SerializeField] float LowJumpMultiplier = 2f;

    [Header("Dashing Variables")]
    [Range(1f,5f)]
    [SerializeField] float DashDistance;
    [SerializeField] float DashCooldown;
    [SerializeField] Color Color;

    MovementState State = MovementState.Idle;
    bool IsHoldingJump = false;
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if(Input.GetKeyDown(KeyCode.F))
        {

        }
    }

    private void Jump()
    {
        rb.velocity = JumpSpeed * Vector2.up;
    }

    private void FixedUpdate()
    {
        //rb.AddForce(Time.fixedDeltaTime * MovementSpeed * MovementAxis, ForceMode2D.Impulse);
        rb.velocity = new Vector2(MovementAxis.x * MovementSpeed, rb.velocity.y);
        ForceMaxSpeed();        
        UpdateAnimationState();           
        if(rb.velocity.y < 0)
        {
            rb.AddForce(Physics2D.gravity.y * FallMultiplier * Time.fixedDeltaTime * Vector2.up, ForceMode2D.Impulse);
            //rb.velocity += Physics2D.gravity.y * (FallMultiplier - 1) * Time.deltaTime * Vector2.up;
        }
        else if(rb.velocity.y > 0 && !IsHoldingJump)
        {
            rb.AddForce(Physics2D.gravity.y * LowJumpMultiplier * Time.fixedDeltaTime * Vector2.up, ForceMode2D.Impulse);
            //rb.velocity += Physics2D.gravity.y * (LowJumpMultiplier - 1) * Time.deltaTime * Vector2.up;
        }
    }

    private void ForceMaxSpeed()
    {
        if (rb.velocity.x > MaxSpeed)
            rb.velocity = new Vector2(MaxSpeed, rb.velocity.y);
        else if(rb.velocity.x < -MaxSpeed)
            rb.velocity = new Vector2(-MaxSpeed, rb.velocity.y);
    }

    private void UpdateAnimationState()
    {
        if(rb.velocity.x > 0.01f)
        {
            State = MovementState.Walking;
            SpriteRenderer.flipX = false;
        }
        else if(rb.velocity.x < -0.01f)
        {
            State = MovementState.Walking;
            SpriteRenderer.flipX = true;
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

        Animator.SetInteger("State", (int)State);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MovementAxis = new Vector2(context.ReadValue<float>(), 0f);
    }
    
    enum MovementState
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Attacking,
        Sliding,
        Dashing
    }
}
