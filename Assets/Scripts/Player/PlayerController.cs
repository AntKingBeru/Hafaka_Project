using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerDash))]
[RequireComponent(typeof(PlayerPlatformDetector))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerDash dash;
    [SerializeField] private PlayerPlatformDetector detector;

    private void Awake()
    {
        input.onMove.AddListener(movement.SetMoveInput);
        input.onJump.AddListener(jump.TryJump);
        input.onDash.AddListener(() => dash.TryDash(movement.LastDirection));
    }

    private void OnDestroy()
    {
        if (input)
        {
            input.onMove.RemoveListener(movement.SetMoveInput);
            input.onJump.RemoveListener(jump.TryJump);
        }
    }

    private void Update()
    {
        var grounded = controller.isGrounded || detector.IsGrounded;
        jump.SetGrounded(grounded);
        
        var mover = detector.CurrentMover;

        var horizontal = dash.IsDashing
            ? dash.DashVelocity
            : movement.HorizontalVelocity;

        var motion = new Vector3(horizontal, jump.VerticalVelocity, 0f) * Time.unscaledDeltaTime;

        if (mover)
            motion.x += mover.DeltaMovement.x;
                
        controller.Move(motion);

        if (mover && mover.DeltaMovement.y != 0f)
        {
            controller.enabled = false;
            transform.position += new Vector3(0f, mover.DeltaMovement.y, 0f);
            controller.enabled = true;
        }
    }
}