using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public enum Axis
    {
        Horizontal,
        Vertical
    }
    
    [Header("Movement Settings")]
    [SerializeField] private Axis axis = Axis.Horizontal;
    [SerializeField] private float distance = 4f;
    [SerializeField] private float speed = 3f;

    private Vector3 _originPosition;
    private float _travelled;
    private float _direction = 1f;
    private int _lastTickedFrame = -1;
    
    public Axis MovementAxis => axis;
    
    public Vector3 DeltaMovement { get; private set; }

    private void Start()
    {
        _originPosition = transform.position;
    }

    public void Tick(float deltaTime)
    {
        if (_lastTickedFrame == Time.frameCount)
            return;
        
        _lastTickedFrame = Time.frameCount;
        
        _travelled += _direction * speed * deltaTime;

        if (_travelled >= distance)
        {
            _travelled = distance;
            _direction = -1f;
        }
        else if (_travelled <= -distance)
        {
            _travelled = -distance;
            _direction = 1f;
        }

        var offset = axis == Axis.Horizontal
            ? new Vector3(_travelled, 0f, 0f)
            : new Vector3(0f, _travelled, 0f);
        
        var newPosition = _originPosition + offset;
        DeltaMovement = newPosition - transform.position;
        transform.position = newPosition;
    }
}