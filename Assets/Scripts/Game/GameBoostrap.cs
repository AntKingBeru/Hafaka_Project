using UnityEngine;

public static class GameBoostrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        var prefab = Resources.Load<GameObject>($"GameManager");

        if (!prefab)
        {
            Debug.LogError("GameBootstrap: PersistentManager prefab not found in Resources.");
            return;
        }

        Object.Instantiate(prefab);
    }
}