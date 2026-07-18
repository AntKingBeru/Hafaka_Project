using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorBridge : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerGodMode godMode;
    
    [Header("Parameter Names")]
    [SerializeField] private string godModeBool = "GodMode";
    [SerializeField] private string jumpStartTrigger = "JumpStart";
    [SerializeField] private string jumpEndTrigger = "JumpEnd";
    [SerializeField] private string dashStartTrigger = "StartDash";
    [SerializeField] private string dashEndTrigger = "EndDash";
    [SerializeField] private string deathTrigger = "Dead";
    
    private readonly List<int> _triggerHashes = new List<int>();
    private readonly HashSet<int> _boolHashes = new HashSet<int>();
    private readonly HashSet<int> _floatHashes = new HashSet<int>();
    
    private bool IsSuppressed => godMode && godMode.IsActive;

    private void Awake()
    {
        CacheParameters();
    }

    private void OnEnable()
    {
        if (godMode)
            godMode.onGodModeChanged.AddListener(HandleGodModeChanged);
    }
    
    private void OnDisable()
    {
        if (godMode)
            godMode.onGodModeChanged.RemoveListener(HandleGodModeChanged);
    }
    
    public void PlayJumpStart()
    {
        if (IsSuppressed)
            return;

        ResetTriggerInternal(jumpStartTrigger);
        ResetTriggerInternal(jumpEndTrigger);
        SetTriggerInternal(jumpStartTrigger);
    }
    
    public void PlayJumpEnd()
    {
        if (IsSuppressed)
            return;

        ResetTriggerInternal(jumpStartTrigger);
        SetTriggerInternal(jumpEndTrigger);
    }

    public void PlayDashStart()
    {
        if (IsSuppressed)
            return;

        ResetTriggerInternal(dashEndTrigger);
        SetTriggerInternal(dashStartTrigger);
    }

    public void PlayDashEnd()
    {
        if (IsSuppressed)
            return;

        ResetTriggerInternal(dashStartTrigger);
        SetTriggerInternal(dashEndTrigger);
    }
    
    public void PlayDeath()
    {
        if (godMode && godMode.IsActive)
            godMode.SetActive(false);

        ClearAllTriggers();
        SetTriggerInternal(deathTrigger);
    }
    
    public void SetTrigger(string triggerName)
    {
        if (IsSuppressed)
            return;
        SetTriggerInternal(triggerName);
    }
    
    public void ResetTrigger(string triggerName) => ResetTriggerInternal(triggerName);
    
    public void SetFloat(string floatName, float value)
    {
        if (!animator || string.IsNullOrEmpty(floatName))
            return;

        var hash = Animator.StringToHash(name);
        if (!_floatHashes.Contains(hash))
            return;

        animator.SetFloat(hash, value);
    }
    
    private void SetTriggerInternal(string triggerName)
    {
        if (!animator || string.IsNullOrEmpty(triggerName))
            return;

        var hash = Animator.StringToHash(triggerName);
        if (!_triggerHashes.Contains(hash))
            return;

        animator.SetTrigger(hash);
    }

    private void ResetTriggerInternal(string triggerName)
    {
        if (!animator || string.IsNullOrEmpty(triggerName))
            return;

        var hash = Animator.StringToHash(name);
        if (!_triggerHashes.Contains(hash))
            return;

        animator.ResetTrigger(hash);
    }

    private void HandleGodModeChanged(bool active)
    {
        if (!animator)
            return;

        if (active)
            ClearAllTriggers();

        if (string.IsNullOrEmpty(godModeBool))
            return;

        var hash = Animator.StringToHash(godModeBool);
        if (_boolHashes.Contains(hash))
            animator.SetBool(hash, active);
    }

    private void ClearAllTriggers()
    {
        foreach (var hash in _triggerHashes)
            animator.ResetTrigger(hash);
    }

    private void CacheParameters()
    {
        if (!animator || !animator.runtimeAnimatorController)
        {
            Debug.LogWarning("PlayerAnimatorBridge: no Animator or controller assigned.");
            return;
        }

        foreach (var p in animator.parameters)
        {
            switch (p.type)
            {
                case AnimatorControllerParameterType.Trigger:
                    _triggerHashes.Add(p.nameHash);
                    break;
                case AnimatorControllerParameterType.Bool:
                    _boolHashes.Add(p.nameHash);
                    break;
                case AnimatorControllerParameterType.Float:
                    _floatHashes.Add(p.nameHash);
                    break;
            }
        }
    }
}