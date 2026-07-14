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
    
    [Header("Events")]
    public UnityEvent onFirstJump;
    public UnityEvent onSecondJump;
    public UnityEvent onLand;

    private int _jumpsRemaining;
    private bool _wasGrounded;
    
    public float VerticalVelocity { get; private set; }
    public bool IsGrounded { get; private set; }

    private void Update()
    {
        if (IsGrounded && VerticalVelocity < 0f)
        {
            VerticalVelocity = -2f;
            return;
        }

        ApplyGravity();
    }
    
    public void SetGrounded(bool grounded)
    {
        if (grounded && !_wasGrounded)
        {
            _jumpsRemaining = 2;
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