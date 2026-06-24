using UnityEngine;
using UnityEngine.Events;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float visibleDuration = 2f;
    [SerializeField] private float invisibleDuration = 1.5f;
    [SerializeField] private bool startVisible = true;
    
    [Header("Events")]
    public UnityEvent onBecomeVisible;
    public UnityEvent onBecomeInvisible;
    
    [Header("References")]
    [SerializeField] private Collider col;
    [SerializeField] private Renderer rend;
    
    private bool _isVisible;
    private float _timer;

    private void Start()
    {
        SetVisible(startVisible);
        _timer = _isVisible ? visibleDuration : invisibleDuration;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer > 0f)
            return;

        SetVisible(!_isVisible);
        _timer = _isVisible ? visibleDuration : invisibleDuration;
    }

    private void SetVisible(bool visible)
    {
        _isVisible = visible;
        
        if (col)
            col.enabled = visible;
        if (rend)
            rend.enabled = visible;
        
        if (visible)
            onBecomeVisible.Invoke();
        else
            onBecomeInvisible.Invoke();
    }
}