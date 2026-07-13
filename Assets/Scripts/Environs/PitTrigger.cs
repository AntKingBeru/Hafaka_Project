using UnityEngine;

public class PitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        other.GetComponent<PlayerHealth>()?.Kill();
    }
}