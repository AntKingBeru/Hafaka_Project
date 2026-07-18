using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [Tooltip("Z offset from the player. Negative = behind the player on the z axis.")]
    [SerializeField] private float zOffset = -20f;
    [SerializeField] private Vector2 xyOffset = new Vector2(0f, 1.5f);
    
    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.18f;
    
    [Header("Look-Ahead")]
    [Tooltip("Camera leads the player horizontally bt his many units.")]
    [SerializeField] private float lookAheadX = 2f;
    
    [Header("Zoom Cheat")]
    [Tooltip("Z offset used while zoomed out. Should be more negative than zOffset.")]
    [SerializeField] private float zoomedOutZOffset = -45f;
    [Tooltip("Seconds it takes to blend between the two z offsets.")]
    [SerializeField] private float zoomBlendDuration = 0.25f;
    [Tooltip("Orthographic cameras only: orthographic size while zoomed out.")]
    [SerializeField] private float zoomedOutOrthoSize = 18f;
    [SerializeField] private InputActionReference zoomToggleAction;
    
    private Vector3 _velocity =  Vector3.zero;
    private float _lastFacingX = 1f;
    
    private bool  _isZoomedOut;
    private float _currentZOffset;
    private float _normalOrthoSize;
    private float _currentOrthoSize;
    
    private bool IsOrthographic => cam && cam.orthographic;
    
    private void Awake()
    {
        _currentZOffset = zOffset;
        
        if (cam)
        {
            _normalOrthoSize  = cam.orthographicSize;
            _currentOrthoSize = _normalOrthoSize;
        }
    }

    private void OnEnable()
    {
        if (!zoomToggleAction)
            return;

        zoomToggleAction.action.Enable();
        zoomToggleAction.action.performed += OnZoomToggled;
    }

    private void OnDisable()
    {
        if (!zoomToggleAction)
            return;

        zoomToggleAction.action.performed -= OnZoomToggled;
        zoomToggleAction.action.Disable();
    }
    
    private void OnZoomToggled(InputAction.CallbackContext ctx)
    {
        _isZoomedOut = !_isZoomedOut;
    }

    private void LateUpdate()
    {
        if (!target)
            return;
        
        BlendZoom();

        var facingX = Mathf.Sign(target.localScale.x);
        if (facingX != 0f)
            _lastFacingX = facingX;

        var desired = new Vector3(
            target.position.x + xyOffset.x * _lastFacingX * lookAheadX,
            target.position.y + xyOffset.y,
            target.position.z + _currentZOffset
        );

        desired.y = Mathf.Max(desired.y, PitSpawner.PitY + xyOffset.y);
        
        transform.position = Vector3.SmoothDamp(
            transform.position, desired, ref _velocity,
            smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
    }
    
    private void BlendZoom()
    {
        var step = zoomBlendDuration > 0f
            ? Time.unscaledDeltaTime / zoomBlendDuration
            : 1f;
        
        if (IsOrthographic)
        {
            var targetSize = _isZoomedOut ? zoomedOutOrthoSize : _normalOrthoSize;
            var range = Mathf.Abs(zoomedOutOrthoSize - _normalOrthoSize);

            _currentOrthoSize = Mathf.MoveTowards(
                _currentOrthoSize, targetSize, range * step);

            cam.orthographicSize = _currentOrthoSize;
        }
        else
        {
            var targetZ = _isZoomedOut ? zoomedOutZOffset : zOffset;
            var range = Mathf.Abs(zoomedOutZOffset - zOffset);

            _currentZOffset = Mathf.MoveTowards(
                _currentZOffset, targetZ, range * step);
        }
    }
}