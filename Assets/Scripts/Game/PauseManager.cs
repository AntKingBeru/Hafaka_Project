using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    
    public static bool IsPaused { get; private set; }
    
    [Header("Events")]
    public UnityEvent<bool> onPauseChanged;

    private float _previousTimeScale = 1f;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
     public void SetPaused(bool paused)
    {
        if (IsPaused == paused)
            return;

        IsPaused = paused;

        if (paused)
        {
            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
            Time.timeScale = _previousTimeScale;

        onPauseChanged.Invoke(paused);
    }

    public void TogglePaused() => SetPaused(!IsPaused);
    
    public static void ClearPause()
    {
        IsPaused = false;

        if (Instance)
            Instance._previousTimeScale = 1f;
    }
}