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
    private float _dashVelocity;

    public bool IsDashing => _isDashing;
    public float DashVelocity => _dashVelocity;

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        if (!_isDashing)
            return;
        
        _dashTimer -= Time.deltaTime;

        if (_dashTimer <= 0f)
            EndDash();
    }

    public void TryDash(int dir)
    {
        if (_isDashing || _cooldownTimer > 0f)
            return;

        _isDashing = true;
        _dashVelocity = dashSpeed * dir;
        _dashTimer = dashDuration;
        onDashStarted.Invoke();
    }

    private void EndDash()
    {
        _isDashing = false;
        _cooldownTimer = dashCooldown;
        _dashVelocity = 0f;
        onDashEnded.Invoke();
    }
}