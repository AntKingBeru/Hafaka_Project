using UnityEngine;

public abstract class PlayerForm : MonoBehaviour
{
    protected PlayerController Player;
    
    public virtual void Initialize(PlayerController player) => Player = player;
    public virtual void OnFormEnter() { }
    public virtual void OnFormExit() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public abstract void OnActive1Pressed();
    public abstract void OnActive2Pressed();
    public virtual float GetSpeedMultiplier() => 1f;
    public virtual float GetDamageMultiplier() => 1f;
    public virtual int GetExtraJumpCharges() => 0;
    public virtual void OnAnyAbilityUsed() { }
}