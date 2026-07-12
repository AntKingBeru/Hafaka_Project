using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerPlatformDetector : MonoBehaviour
{
    [SerializeField] private float raycastPadding = 0.1f;
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerHealth health;

    private GameObject _lastPlatform;
    
    private void Update()
    {
        var rayLength = (controller.height / 2f) + controller.skinWidth + raycastPadding;
        var origin = transform.position + Vector3.up * (controller.height / 2f);

        if (!Physics.Raycast(origin, Vector3.down, out var hit, rayLength))
        {
            _lastPlatform = null;
            return;
        }

        NotifyContact(hit.collider);
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
        other.GetComponent<SpikeTrap>()?.OnPlayerLanded(health);

        var platform = other.gameObject;
        if (platform == _lastPlatform)
            return;
        
        _lastPlatform = platform;
        other.GetComponent<CrumblingPlatform>()?.OnPlayerLanded();
    }
}