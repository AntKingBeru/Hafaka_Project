using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerPlatformDetector))]
[RequireComponent(typeof(PlayerGodMode))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerDash dash;
    [SerializeField] private PlayerPlatformDetector detector;
    [SerializeField] private PlayerGodMode godMode;

    private bool _isFlying;

    private void Awake()
    {
        input.onMove.AddListener(movement.SetMoveInput);
        input.onJump.AddListener(jump.TryJump);
        input.onDash.AddListener(() => dash.TryDash(movement.LastDirection));
    }
    
    private void OnEnable()
    {
        if (godMode)
            godMode.onGodModeChanged.AddListener(HandleGodModeChanged);
    }

    private void OnDisable()
    {
        if (godMode)
            godMode.onGodModeChanged.RemoveListener(HandleGodModeChanged);
    }

    private void OnDestroy()
    {
        if (input)
        {
            input.onMove.RemoveListener(movement.SetMoveInput);
            input.onJump.RemoveListener(jump.TryJump);
        }
    }
    
    private void HandleGodModeChanged(bool active)
    {
        _isFlying = active;

        jump.ResetVerticalVelocity();

        if (!active)
            jump.RefillJumps();
    }

    private void Update()
    {
        if (PauseManager.IsPaused)
            return;
        
        var grounded = controller.isGrounded || detector.IsGrounded;
        jump.SetGrounded(grounded);
        
        var mover = detector.CurrentMover;

        var horizontal = dash.IsDashing
            ? dash.DashVelocity
            : movement.HorizontalVelocity;
        
        float vertical;

        if (_isFlying)
        {
            horizontal *= godMode.SpeedMultiplier;
            
            jump.ResetVerticalVelocity();
            vertical = movement.VerticalInput * godMode.FlightSpeed;
        }
        else
            vertical = jump.VerticalVelocity;

        var motion = new Vector3(horizontal, vertical, 0f) * Time.unscaledDeltaTime;

        if (mover && !_isFlying)
            motion.x += mover.DeltaMovement.x;
                
        controller.Move(motion);

        if (mover && mover.DeltaMovement.y != 0f && !_isFlying)
        {
            controller.enabled = false;
            transform.position += new Vector3(0f, mover.DeltaMovement.y, 0f);
            controller.enabled = true;
        }
    }
}