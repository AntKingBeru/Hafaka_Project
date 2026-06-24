using UnityEngine;
using UnityEngine.Events;

public class CrumblingPlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float crumbleDelay = 0.6f;
    [SerializeField] private float respawnDelay = 3f;
    
    [Header("Events")]
    public UnityEvent onCrumbleWarning;
    public UnityEvent onCrumble;
    public UnityEvent onRespawn;
    
    [Header("References")]
    [SerializeField] private Collider col;
    [SerializeField] private Renderer[] rends;

    private bool _isCrumbling;
    private bool _isBroken;
    private float _timer;

    public void OnPlayerLanded()
    {
        if (_isCrumbling || _isBroken)
            return;
        
        _isCrumbling = true;
        _timer = crumbleDelay;
        onCrumbleWarning.Invoke();
    }

    private void Update()
    {
        if (!_isCrumbling && !_isBroken)
            return;
        
        _timer -= Time.deltaTime;

        if (_isCrumbling && _timer <= 0f)
        {
            Crumble();
            return;
        }

        if (_isBroken && _timer <= 0f)
            Respawn();
    }

    private void Crumble()
    {
        _isCrumbling = false;
        _isBroken = true;
        _timer = respawnDelay;
        
        if (col)
            col.enabled = false;
        if (rends.Length > 0)
            foreach (var rend in rends)
                rend.enabled = false;
        
        onCrumble.Invoke();
    }

    private void Respawn()
    {
        _isBroken = false;
        
        if (col)
            col.enabled = true;
        if (rends.Length > 0)
            foreach (var rend in rends)
                rend.enabled = true;
        
        onRespawn.Invoke();
    }
}