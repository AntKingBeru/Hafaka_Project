using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPlatformDetector : MonoBehaviour
{
    [SerializeField] private float raycastPadding = 0.1f;
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private PlayerGodMode godMode;
    [SerializeField] private LayerMask platformMask = Physics.DefaultRaycastLayers;
    [SerializeField] private LayerMask pitMask;

    private GameObject _lastPlatform;
    
    public PlatformMover CurrentMover { get; private set; }
    public bool IsGrounded { get; private set; }

    private void Update()
    {
        CheckGround();
        CheckPit();
    }
    
    private void CheckGround()
    {
        var rayLength = (controller.height / 2f) + controller.skinWidth + raycastPadding;
        var origin = transform.position + Vector3.up * (controller.height / 2f);

        if (!Physics.Raycast(origin, Vector3.down, out var hit, rayLength, platformMask))
        {
            _lastPlatform = null;
            CurrentMover = null;
            IsGrounded = false;
            return;
        }

        IsGrounded = true;
        CurrentMover = hit.collider.GetComponent<PlatformMover>();

        NotifyContact(hit.collider);
    }

    private void CheckPit()
    {
        var rayLength = (controller.height / 2f) + controller.skinWidth + 0.5f;
        var origin = transform.position + Vector3.up * (controller.height / 2f);
        
        if (Physics.Raycast(origin, Vector3.down, out var hit, rayLength, pitMask))
            hit.collider.GetComponent<PitTrigger>()?.TriggerDeath(health);
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0.5f)
            return;
        
        NotifyContact(hit.collider);
    }

    private void NotifyContact(Collider other)
    {
        other.GetComponent<BouncyPlatform>()?.OnPlayerLanded(jump);
        
        if (godMode || !godMode.IsActive)
            other.GetComponent<SpikeTrap>()?.OnPlayerLanded(health);

        var platform = other.gameObject;
        if (platform == _lastPlatform)
            return;
        
        _lastPlatform = platform;
        other.GetComponent<CrumblingPlatform>()?.OnPlayerLanded();
    }
}