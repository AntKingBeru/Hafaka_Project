using UnityEngine;
using UnityEngine.Events;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 13.5f;
    [SerializeField] private float riseGravity = 20f;
    [SerializeField] private float fallGravity = 28f;
    [SerializeField] private float apexGravityBoost = 6f;
    [SerializeField] private float apexThreshold = 2f;
    
    [Header("Head Bump")]
    [SerializeField] private float headBumpVelocity = -1f;
    
    [Header("Events")]
    public UnityEvent onFirstJump;
    public UnityEvent onSecondJump;
    public UnityEvent onLand;

    private const int MaxJumps = 2;

    private int _jumpsRemaining;
    private bool _wasGrounded;
    private int _bouncedFrame = -1;
    
    public float VerticalVelocity { get; private set; }
    public bool IsGrounded { get; private set; }
    
    private bool BouncedThisFrame => _bouncedFrame == Time.frameCount;

    private void Awake()
    {
        _jumpsRemaining = MaxJumps;
    }

    private void Update()
    {
        if (IsGrounded && VerticalVelocity < 0f && !BouncedThisFrame)
        {
            VerticalVelocity = -2f;
            return;
        }

        ApplyGravity();
    }
    
    public void SetGrounded(bool grounded)
    {
        var justLanded = grounded && !_wasGrounded;
        
        if (justLanded && !BouncedThisFrame)
        {
            _jumpsRemaining = MaxJumps;
            VerticalVelocity = 0f;
            onLand.Invoke();
        }
        
        IsGrounded = grounded;
        _wasGrounded = grounded;
    }

    public void TryJump()
    {
        if (_jumpsRemaining <= 0)
            return;
        
        var isFirst = _jumpsRemaining == 2;
        VerticalVelocity = jumpForce;
        _jumpsRemaining--;
        
        if (isFirst)
            onFirstJump.Invoke();
        else
            onSecondJump.Invoke();
    }

    public void ApplyBounce(float force)
    {
        VerticalVelocity = force;

        _jumpsRemaining = MaxJumps - 1;
        _bouncedFrame = Time.frameCount;

        onFirstJump.Invoke();
    }

    public void ResetVerticalVelocity()
    {
        VerticalVelocity = 0f;
    }

    public void RefillJumps()
    {
        _jumpsRemaining = MaxJumps;
    }
    
    public void OnHeadBump()
    {
        if (VerticalVelocity <= 0f)
            return;

        VerticalVelocity = headBumpVelocity;
    }

    private void ApplyGravity()
    {
        float gravity;
        
        var nearApex = Mathf.Abs(VerticalVelocity) < apexThreshold && !IsGrounded;

        if (VerticalVelocity > 0f)
            gravity = riseGravity + (nearApex ? apexGravityBoost : 0f);
        else
            gravity = fallGravity;
        
        VerticalVelocity -= gravity * Time.unscaledDeltaTime;
    }
}