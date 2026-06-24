using UnityEngine;
using UnityEngine.Events;

public class BouncyPlatform : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 20f;
    
    [Header("Events")]
    public UnityEvent onBounce;

    public void OnPlayerLanded(PlayerJump jump)
    {
        jump.ApplyBounce(bounceForce);
        onBounce.Invoke();
    }
}