using UnityEngine;

public class PlatformMoverManager : MonoBehaviour
{
    private PlatformMover[] _movers;

    private void Start()
    {
        _movers = FindObjectsByType<PlatformMover>(FindObjectsInactive.Exclude);
    }

    private void Update()
    {
        foreach (var mover in _movers)
            mover.Tick(Time.deltaTime);
    }
}