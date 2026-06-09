using UnityEngine;

public class Traversal : PlayerForm
{
    [Header("Passive")]
    [SerializeField] private float speedBoostMultiplier = 1.4f;
    [SerializeField] private float speedBoostDuration = 1.2f;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 18f;
    [SerializeField] private float dashCooldown = 0.8f;

    private float _speedBoostEndTime;
    private float _lastDashTime = -999f;

    public override void OnFormExit()
    {
        _speedBoostEndTime = 0f;
    }
    
    public override float GetSpeedMultiplier()
        => (Time.time < _speedBoostEndTime) ? speedBoostMultiplier : 1f;
    
    public override void OnAnyAbilityUsed() 
        => _speedBoostEndTime = Time.time + speedBoostDuration;

    public override int GetExtraJumpCharges() => 1;

    public override void OnActive1Pressed() { }
    public override void OnActive2Pressed() => TryDash();

    private void TryDash()
    {
        if (Time.time - _lastDashTime < dashCooldown)
            return;
        _lastDashTime = Time.time;

        Player.Rb.linearVelocity = new Vector2(
            Player.FacingDirection * dashSpeed,
            Player.Rb.linearVelocity.y
        );
        
        OnAnyAbilityUsed();
        Debug.Log("[TraversalForm] Dash!");
    }
}