using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformMoverManager : MonoBehaviour
{
    private PlatformMover[] _movers;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _movers = FindObjectsByType<PlatformMover>(FindObjectsInactive.Exclude);
    }

    private void Update()
    {
        foreach (var mover in _movers)
            mover.Tick(Time.deltaTime);
    }
}