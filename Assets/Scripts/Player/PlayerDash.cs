using UnityEngine;
using UnityEngine.Events;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 22f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 0.8f;
    
    [Header("Events")]
    public UnityEvent onDashStarted;
    public UnityEvent onDashEnded;

    private bool _isDashing;
    private float _dashTimer;
    private float _cooldownTimer;
    private int _dashDirection;

    public float DashVelocity { get; private set; } = float.NaN;
    public bool IsDashing => _isDashing;

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (!_isDashing)
        {
            DashVelocity = float.NaN;
            return;
        }
        
        _dashTimer -= Time.deltaTime;

        if (_dashTimer <= 0f)
            EndDash();
        else
            DashVelocity = dashSpeed * _dashDirection;
    }

    public void TryDash(int dir)
    {
        if (!_isDashing || _cooldownTimer > 0f)
            return;

        _isDashing = true;
        _dashDirection = dir;
        _dashTimer = dashDuration;
        DashVelocity = dashSpeed * _dashDirection;
        onDashStarted.Invoke();
    }

    private void EndDash()
    {
        _isDashing = false;
        _cooldownTimer = dashCooldown;
        DashVelocity = float.NaN;
        onDashEnded.Invoke();
    }
}