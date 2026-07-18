using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    
    [Header("Visual Root")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float rotationDuration = 0.08f;

    [SerializeField] private Animator anim;

    private int _lastDirection = 1;
    private float _currentInput;

    private bool _isRotating;
    private float _rotationTimer;
    private float _rotationStartY;
    private float _rotationEndY;
    private float _currentVerticalInput;

    public float HorizontalVelocity { get; private set; }
    
    public int LastDirection => _lastDirection;
    public float VerticalInput => _currentVerticalInput;

    private void Update()
    {
        HorizontalVelocity = _currentInput * moveSpeed;

        switch (_currentInput)
        {
            case > 0f when _lastDirection != 1:
                SetFacing(1);
                break;
            case < 0f when _lastDirection != -1:
                SetFacing(-1);
                break;
        }

        TickRotation();
    }
    
    public void SetMoveInput(Vector2 input)
    {
        _currentInput = input.x;
        _currentVerticalInput = input.y;
        anim.SetFloat(MoveSpeed, Mathf.Abs(input.x));
    }

    private void SetFacing(int dir)
    {
        _lastDirection = dir;
        
        _rotationEndY = dir == 1 ? 0f : 180f;

        var currentY = visualRoot
            ? visualRoot.localEulerAngles.y
            : 0f;
        
        currentY = (currentY % 360f + 360f) % 360f;
        
        _rotationStartY = currentY;
        _rotationTimer = 0f;
        _isRotating = true;
    }

    private void TickRotation()
    {
        if (!_isRotating || !visualRoot)
            return;
        
        _rotationTimer += Time.deltaTime;
        var t = Mathf.Clamp01(_rotationTimer / rotationDuration);
        
        var smoothT = Mathf.SmoothStep(0f, 1f, t);
        
        var angle = Mathf.LerpAngle(_rotationStartY, _rotationEndY, smoothT);
        visualRoot.localEulerAngles = new Vector3(0f, angle, 0f);

        if (t >= 1f)
        {
            var snapped = (Mathf.RoundToInt(_rotationEndY) % 360 + 360) % 360;
            visualRoot.localEulerAngles = new Vector3(0f, snapped, 0f);
            _isRotating = false;
        }
    }
}