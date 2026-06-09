using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    public enum GrappleState
    {
        Idle,
        Firing,
        Pulling,
        Attached
    }
    
    [Header("Grapple Settings")]
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private float maxGrappleLength = 10f;
    [SerializeField] private float maxRopeLength = 6f;
    [SerializeField] private LayerMask grappleTargets;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LineRenderer lineRenderer;

    public GrappleState State { get; private set; } = GrappleState.Idle;

    private Vector3 _hookPosition;
    private Vector3 _fireDirection;
    private float _firedDistance;
    private PlayerController _player;

    public void Initialize(PlayerController player) => _player = player;

    public bool Toggle(Vector3 worldTarget)
    {
        switch (State)
        {
            case GrappleState.Idle:
                Fire(worldTarget);
                return true;
            case GrappleState.Firing:
            case GrappleState.Attached:
                Cancel();
                return true;
            case GrappleState.Pulling:
            default:
                return false;
        }
    }

    public void Cancel()
    {
        State = GrappleState.Idle;
        if (lineRenderer)
            lineRenderer.enabled = false;
    }

    private void Fire(Vector3 worldTarget)
    {
        _hookPosition = _player.transform.position;
        var toTarget = worldTarget - _hookPosition;
        toTarget.z = 0f;
        _fireDirection = toTarget.normalized;
        _firedDistance = 0f;
        State = GrappleState.Firing;
        if (lineRenderer)
            lineRenderer.enabled = true;
    }

    private void Update()
    {
        switch (State)
        {
            case GrappleState.Firing:
                UpdateFiring();
                break;
            case GrappleState.Pulling:
                UpdatePulling();
                break;
            case GrappleState.Attached:
                UpdateAttached();
                break;
            case GrappleState.Idle:
            default:
                break;
        }
        UpdateLineRenderer();
    }

    private void UpdateFiring()
    {
        var step = projectileSpeed * Time.deltaTime;
        var didHit = Physics.Raycast(_hookPosition, _fireDirection, out var hit, step, grappleTargets | enemyLayer);

        if (didHit)
        {
            if (((1 << hit.collider.gameObject.layer) & enemyLayer) != 0)
            {
                Cancel();
                return;
            }
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Interact();
                Cancel();
                return;
            }
            _hookPosition = hit.point;
            _hookPosition.z = 0f;
            State = GrappleState.Pulling;
            return;
        }
        _hookPosition += _fireDirection * step;
        _firedDistance += step;
        if (_firedDistance >= maxGrappleLength)
            Cancel();
    }

    private void UpdatePulling()
    {
        var toAnchor = _hookPosition - _player.transform.position;
        var dist = toAnchor.magnitude;

        if (dist > 0.2f)
        {
            var pullDir = toAnchor.normalized;
            var targetVel = pullDir * projectileSpeed;
            targetVel.z = 0f;
            _player.Rb.linearVelocity = Vector3.Lerp(_player.Rb.linearVelocity, targetVel, Time.deltaTime * 8f);
        }
        else
        {
            State = GrappleState.Attached;
        }
    }

    private void UpdateAttached()
    {
        var toPlayer = _player.transform.position - _hookPosition;
        var dist = toPlayer.magnitude;

        if (dist > maxRopeLength)
        {
            var radial = toPlayer.normalized;
            var outwardSpeed = Vector3.Dot(_player.Rb.linearVelocity, radial);
            if (outwardSpeed > 0f)
                _player.Rb.linearVelocity -= radial * outwardSpeed;
            var corrected = _hookPosition + radial * maxRopeLength;
            corrected.z = 0f;
            _player.Rb.MovePosition(corrected);
        }
        
        var vel = _player.Rb.linearVelocity;
        vel.z = 0f;
        _player.Rb.linearVelocity = vel;
    }

    private void UpdateLineRenderer()
    {
        if (!lineRenderer || State == GrappleState.Idle)
            return;
        lineRenderer.SetPosition(0, _player.transform.position);
        lineRenderer.SetPosition(1, _hookPosition);
    }

    private void OnDrawGizmosSelected()
    {
        if (State is GrappleState.Attached or GrappleState.Pulling)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_hookPosition, 0.15f);
            Gizmos.DrawLine(transform.position, _hookPosition);
        }
    }
}