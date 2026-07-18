using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent onDeathSilent;
    public UnityEvent onWin;
    
    [SerializeField] private PlayerInputHandler input;

    private bool _isDead;

    public void Kill(bool playVFX = true)
    {
        if (_isDead)
            return;

        _isDead = true;
        input.enabled = false;
        Time.timeScale = 0f;
        
        if (playVFX)
            onDeath.Invoke();
        else
            onDeathSilent.Invoke();
    }
}