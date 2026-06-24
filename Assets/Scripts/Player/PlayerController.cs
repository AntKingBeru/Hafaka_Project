using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerDash))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerJump jump;
    [SerializeField] private PlayerDash dash;

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
        jump.SetGrounded(controller.isGrounded);

        var horizontal = dash.IsDashing
            ? dash.DashVelocity
            : movement.HorizontalVelocity;

        var motion = new Vector3(horizontal, jump.VerticalVelocity, 0f) * Time.deltaTime;
        
        controller.Move(motion);
    }
}