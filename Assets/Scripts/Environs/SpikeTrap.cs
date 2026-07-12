using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public void OnPlayerLanded(PlayerHealth health)
    {
        health.Kill();
    }
}