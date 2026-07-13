using UnityEngine;

public class PitSpawner : MonoBehaviour
{
    [Tooltip("How far below the lowest renderer the pit trigger sits.")]
    [SerializeField] private float yOffset = -10f;

    [Tooltip("Z scale of the pit trigger volume.")]
    [SerializeField] private float zScale = 10f;

    [Tooltip("Extra width added to each side beyond the map bounds.")]
    [SerializeField] private float xPadding = 50f;
    
    public static float PitY { get; private set; }

    private void Start()
    {
        SpawnPit();
    }

    private void SpawnPit()
    {
        var renderers = FindObjectsByType<Renderer>(FindObjectsInactive.Exclude);

        if (renderers.Length == 0)
        {
            Debug.LogWarning("PitSpawner: No renderers found to calculate map bounds.");
            return;
        }

        var minX = float.MaxValue;
        var maxX = float.MinValue;
        var minY = float.MaxValue;

        foreach (var r in renderers)
        {
            minX = Mathf.Min(minX, r.bounds.min.x);
            maxX = Mathf.Max(maxX, r.bounds.max.x);
            minY = Mathf.Min(minY, r.bounds.min.y);
        }

        var centerX = (minX + maxX) / 2f;
        var width = (maxX - minX) + (xPadding * 2f);
        PitY = minY + yOffset;
        
        var pit = new GameObject("PitTrigger")
        {
            tag = "Untagged",
            layer = LayerMask.NameToLayer("PlayerInteract"),
            transform =
            {
                position = new Vector3(centerX, PitY, 0f)
            }
        };

        var col  = pit.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(width, 1f, zScale);

        pit.AddComponent<PitTrigger>();
    }
}