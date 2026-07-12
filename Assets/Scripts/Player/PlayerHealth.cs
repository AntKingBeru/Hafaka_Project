using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onDeath;
    
    [SerializeField] private PlayerController controller;

    private bool _isDead;

    public void Kill()
    {
        if (_isDead)
            return;

        _isDead = true;
        controller.enabled = false;
        Time.timeScale = 0f;
        
        Debug.Log("Player died.");
        onDeath.Invoke();
    }
}