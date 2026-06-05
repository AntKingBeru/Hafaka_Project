using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    [SerializeField] private float maxMoveSpeed = 8f;
    [SerializeField] private float accelerationFactor = 10f;
    [SerializeField] private float decelerationFactor = 15f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float coyoteTime = 0.12f;

    [Header("Ground Check Settings")]
    [SerializeField] private Vector3 groundCheckOrigin = new(0f, 0.05f, 0f);
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Input Actions")] [SerializeField]
    private InputActionReference moveAction;

    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference active1Action;
    [SerializeField] private InputActionReference active2Action;
    [SerializeField] private InputActionReference formMeleeAction;
    [SerializeField] private InputActionReference formRangedAction;
    [SerializeField] private InputActionReference formTraversalAction;

    [Header("References")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    
    public Rigidbody Rb => rb;
    public bool IsGrounded { get; private set; }
    public int FacingDirection { get; private set; } = 1;
    public int BaseJumpCharges { get; private set; } = 1;
    public int RemainingJumpCharges { get; private set; }

    private float _lastGroundedTime;
    private bool _jumpConsumed;
    private bool _wasGroundedLastFrame;

    private void Awake()
    {
        RemainingJumpCharges = BaseJumpCharges;
    }

    private void Start()
    {
        FormManager.Instance.Initialize(this);
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        active1Action.action.Enable();
        active2Action.action.Enable();
        formMeleeAction.action.Enable();
        formRangedAction.action.Enable();
        formTraversalAction.action.Enable();

        jumpAction.action.performed += OnJumpPerformed;
        active1Action.action.performed += OnActive1Performed;
        active2Action.action.performed += OnActive2Performed;
        formMeleeAction.action.performed += _ => FormManager.Instance.SwitchToForm(0);
        formRangedAction.action.performed += _ => FormManager.Instance.SwitchToForm(1);
        formTraversalAction.action.performed += _ => FormManager.Instance.SwitchToForm(2);
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJumpPerformed;
        active1Action.action.performed -= OnActive1Performed;
        active2Action.action.performed -= OnActive2Performed;

        moveAction.action.Disable();
        jumpAction.action.Disable();
        active1Action.action.Disable();
        active2Action.action.Disable();
        formMeleeAction.action.Disable();
        formRangedAction.action.Disable();
        formTraversalAction.action.Disable();
    }

    private void Update()
    {
        UpdateGrounded();
        HandleFlip();
        FormManager.Instance.OnUpdate();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleFallGravity();
        FormManager.Instance.OnFixedUpdate();
    }

    private void UpdateGrounded()
    {
        var groundedThisFrame = Physics.SphereCast(
            transform.position + groundCheckOrigin,
            groundCheckRadius,
            Vector3.down,
            out _,
            groundCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );
        
        if (groundedThisFrame && !_wasGroundedLastFrame)
            ResetJumpCharges();

        if (groundedThisFrame)
            _lastGroundedTime = Time.time;
        
        IsGrounded = groundedThisFrame;
        _wasGroundedLastFrame = groundedThisFrame;
    }

    private void HandleMovement()
    {
        var input = moveAction.action.ReadValue<Vector2>().x;
        var targetX = input * maxMoveSpeed * FormManager.Instance.CurrentForm.GetSpeedMultiplier();

        var currentX = Rb.linearVelocity.x;
        var factor = (Mathf.Abs(input) > 0.01f) ? accelerationFactor : decelerationFactor;
        var newX = Mathf.MoveTowards(currentX, targetX, factor * Time.fixedDeltaTime);

        Rb.linearVelocity = new Vector3(newX, Rb.linearVelocity.y, 0f);
    }

    private void HandleFallGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector3.up
                                 * Physics.gravity.y
                                 * (fallGravityMultiplier - 1f)
                                 * Time.fixedDeltaTime;
        }
    }

    private void HandleFlip()
    {
        var input = moveAction.action.ReadValue<Vector2>().x;

        if (input > 0.1f && FacingDirection != 1)
            SetFacing(1);
        else if (input < -0.1f && FacingDirection != -1)
            SetFacing(-1);
    }

    private void SetFacing(int dir)
    {
        FacingDirection = dir;
        var scale = visualRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        visualRoot.localScale = scale;
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        var withinCoyoteWindow = (Time.time - _lastGroundedTime) <= coyoteTime;
        var canGroundJump = (IsGrounded || withinCoyoteWindow) && RemainingJumpCharges > 0;
        var totalCharges = BaseJumpCharges + FormManager.Instance.CurrentForm.GetExtraJumpCharges();
        var canAirJump = !IsGrounded && !withinCoyoteWindow
                                     && RemainingJumpCharges > 0
                                     && RemainingJumpCharges < totalCharges;
        if (!canGroundJump && !canAirJump)
            return;
        
        _lastGroundedTime = -999f;
        RemainingJumpCharges--;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
        FormManager.Instance.CurrentForm.OnAnyAbilityUsed();
    }

    private void ResetJumpCharges()
    {
        RemainingJumpCharges = BaseJumpCharges + FormManager.Instance.CurrentForm.GetExtraJumpCharges();
    }

    private void OnActive1Performed(InputAction.CallbackContext ctx) => FormManager.Instance.OnActive1Pressed();
    
    private void OnActive2Performed(InputAction.CallbackContext ctx) => FormManager.Instance.OnActive2Pressed();

    public void TakeDamage(float amount)
    {
        var reduced = amount * FormManager.Instance.CurrentForm.GetDamageMultiplier();
        // TODO: hook into health system; log for now
        Debug.Log($"[Player] Took {reduced} damage (raw: {amount})");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + groundCheckOrigin, groundCheckRadius);
        Gizmos.DrawWireSphere(transform.position + groundCheckOrigin + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawLine(
            transform.position + groundCheckOrigin + Vector3.left * groundCheckRadius,
            transform.position + groundCheckOrigin + Vector3.down * groundCheckDistance +
            Vector3.left * groundCheckRadius
        );
        Gizmos.DrawLine(
            transform.position + groundCheckOrigin + Vector3.right * groundCheckRadius,
            transform.position + groundCheckOrigin + Vector3.down * groundCheckDistance +
            Vector3.right * groundCheckRadius
        );
    }
}