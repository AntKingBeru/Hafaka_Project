using UnityEngine;

public class PlatformPlayerCarrier : MonoBehaviour
{
    [SerializeField] private Collider topSurfaceTrigger;
    [SerializeField] private CrumblingPlatform crumblingPlatform;
    [SerializeField] private BouncyPlatform bouncyPlatform;

    private Transform _playerTransform;
    private PlayerJump _playerJump;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        _playerTransform = other.transform;
        _playerJump = other.GetComponent<PlayerJump>();
        
        crumblingPlatform?.OnPlayerLanded();
        
        if (bouncyPlatform && _playerJump)
            bouncyPlatform.OnPlayerLanded(_playerJump);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        if (_playerTransform)
            _playerTransform.SetParent(null);
        
        _playerTransform = null;
        _playerJump = null;
    }
}