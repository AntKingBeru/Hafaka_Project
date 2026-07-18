using UnityEngine;

public class PitTrigger : MonoBehaviour
{
    public void TriggerDeath(PlayerHealth health)
    {
        health?.Kill(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        other.GetComponentInParent<PlayerHealth>()?.Kill(false);
    }
}