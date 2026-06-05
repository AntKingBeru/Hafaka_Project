using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

public class Ranged : PlayerForm
{
    [Header("Passive")]
    [SerializeField] private float highlightRadius = 8f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private int highlightBufferSize = 32;
    
    [Header("Grapple")]
    [SerializeField] private GrappleHook grappleHook;
    
    [Header("Whip")]
    [SerializeField] private float whipRange = 4f;
    [SerializeField] private float whipHalfHeight = 0.2f;
    [SerializeField] private float whipHalfDepth  = 0.5f;
    [SerializeField] private float whipDamage = 15f;
    [SerializeField] private float whipCooldown = 0.6f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private int whipBufferSize = 16;
    
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    private float _lastWhipTime = -999f;
    private readonly HashSet<IHighlightable> _highlighted = new();
    
    private Collider[] _highlightBuffer;
    private Collider[] _whipBuffer;

    private void Awake()
    {
        _highlightBuffer = new Collider[highlightBufferSize];
        _whipBuffer = new Collider[whipBufferSize];
    }

    public override void OnFormEnter()
    {
        grappleHook.Initialize(Player);
    }

    public override void OnFormExit()
    {
        grappleHook.Cancel();
        foreach (var highlight in _highlighted)
            highlight.UnHighlight();
        _highlighted.Clear();
    }

    public override void OnUpdate()
    {
        UpdateHighlights();
    }

    private void UpdateHighlights()
    {
        var nearbyCount = Physics.OverlapSphereNonAlloc(Player.transform.position, highlightRadius, _highlightBuffer, interactableLayer);
        
        var newSet = new HashSet<IHighlightable>();
        for (var i = 0; i < nearbyCount; i++)
            if (_highlightBuffer[i].TryGetComponent<IHighlightable>(out var high))
                newSet.Add(high);
        
        foreach (var highlight in _highlighted.Where(highlight => !newSet.Contains(highlight)))
            highlight.UnHighlight();

        foreach (var highlight in newSet.Where(highlight => !_highlighted.Contains(highlight)))
            highlight.Highlight();

        _highlighted.Clear();
        foreach (var highlight in newSet)
            _highlighted.Add(highlight);
    }

    public override void OnActive1Pressed()
    {
        var mouseScreen = Mouse.current.position.ReadValue();
        var distToPlane = Mathf.Abs(mainCamera.transform.position.z);
        var mouseWorld = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, distToPlane));
        mouseWorld.z = 0f;
        var toggled = grappleHook.Toggle(mouseWorld);
        if (toggled)
            FormManager.Instance.CurrentForm.OnAnyAbilityUsed();
    }

    public override void OnActive2Pressed()
    {
        if (Time.time - _lastWhipTime < whipCooldown)
            return;
        _lastWhipTime = Time.time;

        var centre = Player.transform.position
                         + new Vector3(Player.FacingDirection * (whipRange * 0.5f), 0f, 0f);

        var halfExtents = new Vector3(whipRange * 0.5f, whipHalfHeight, whipHalfDepth);

        var hitCount = Physics.OverlapBoxNonAlloc(centre, halfExtents, _whipBuffer, Quaternion.identity, targetLayers);

        for (var i = 0; i < hitCount; i++)
            if (_whipBuffer[i].TryGetComponent<IDamageable>(out var dmg))
                dmg.TakeDamage(whipDamage);
        
        FormManager.Instance.CurrentForm.OnAnyAbilityUsed();
        Debug.Log("[RangedForm] Whip Attack!");
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!Player)
            return;
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawWireSphere(Player.transform.position, highlightRadius);

        Gizmos.color = Color.magenta;
        var centre = Player.transform.position
                         + new Vector3(Player.FacingDirection * (whipRange * 0.5f), 0f, 0f);
        Gizmos.DrawWireCube(centre, new Vector3(whipRange, whipHalfHeight * 2f, whipHalfDepth * 2f));
    }
}