using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputHandler : MonoBehaviour
{
    private InputActions input;
    private bool clickedJump = false;
    private bool clickedDash = false;
    public bool ClickedAttack { get; private set; } = false;
    private Vector2 movementAxis;

    public bool PressedJump
    {
        get { return clickedJump; }
    }
    public bool PressedDash
    {
        get { return clickedDash; }
    }
    public bool PressedAttack
    {
        get { return ClickedAttack; }
    }

    public Vector2 MovementAxis
    {
        get { return movementAxis; }
    }
    private void Awake()
    {
        input = new InputActions();
        input.ActionMap.Movement.started += OnMove;
        input.ActionMap.Movement.canceled += OnMove;
        input.ActionMap.Movement.performed += OnMove;
        input.ActionMap.Dash.started += OnDash;
        input.ActionMap.Dash.canceled += OnDash;
        input.ActionMap.Jump.started += OnJump;
        input.ActionMap.Jump.canceled += OnJump;
        input.ActionMap.Jump.performed += OnJump;
        input.ActionMap.Attack.started += OnAttack;
        input.ActionMap.Attack.canceled += OnAttack;
    }
    private void OnJump(InputAction.CallbackContext context)
    {
        clickedJump = context.ReadValueAsButton();
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        ClickedAttack = context.ReadValueAsButton();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        movementAxis = context.ReadValue<Vector2>();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        clickedDash = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        input.ActionMap.Enable();
    }
    private void OnDisable()
    {
        input.ActionMap.Disable();
    }
}
