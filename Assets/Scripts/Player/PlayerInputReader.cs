using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference dashAction;
    
    [Header("Events")]
    public UnityEvent<Vector2> onMove;
    public UnityEvent onJump;
    public UnityEvent onDash;
    
    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        dashAction.action.Enable();

        jumpAction.action.performed += HandleJump;
        dashAction.action.performed += HandleDash;
    }
    
    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        dashAction.action.Disable();

        jumpAction.action.performed -= HandleJump;
        dashAction.action.performed -= HandleDash;
    }

    private void Update()
    {
        var moveInput = moveAction.action.ReadValue<Vector2>();
        onMove.Invoke(moveInput);
    }
    
    private void HandleJump(InputAction.CallbackContext ctx) => onJump.Invoke();
    private void HandleDash(InputAction.CallbackContext ctx) => onDash.Invoke();
}