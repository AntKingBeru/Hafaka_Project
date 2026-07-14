using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
[RequireComponent(typeof(PitSpawner))]
[RequireComponent(typeof(PlatformMoverManager))]
public class PersistentManager : MonoBehaviour
{
    private static PersistentManager _instance;

    private void Awake()
    {
        if (_instance && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}