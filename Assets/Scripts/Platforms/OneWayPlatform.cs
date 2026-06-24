using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private float passThroughTolerance = 0.1f;
    [SerializeField] private Collider col;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        var platformTop = col.bounds.max.y;
        var playerBottom = other.bounds.min.y;
        
        var playerIsBelow = playerBottom < platformTop - passThroughTolerance;
        
        Physics.IgnoreCollision(col, other, playerIsBelow);
    }
}