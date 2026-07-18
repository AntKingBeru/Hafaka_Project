using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerGodMode : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Bind as Left Ctrl + Left Shift + G (Two Modifiers composite).")]
    [SerializeField] private InputActionReference toggleAction;

    [Header("Settings")]
    [Tooltip("Horizontal speed multiplier while god mode is active.")]
    [SerializeField] private float speedMultiplier = 1.6f;

    [Tooltip("Vertical flight speed in m/s.")]
    [SerializeField] private float flightSpeed = 10f;

    [Header("Events")]
    [Tooltip("Fires with the new state whenever god mode is toggled.")]
    public UnityEvent<bool> onGodModeChanged;
    public UnityEvent onGodModeEnabled;
    public UnityEvent onGodModeDisabled;
    
    public bool IsActive { get; private set; }

    public float SpeedMultiplier => speedMultiplier;
    public float FlightSpeed => flightSpeed;

    private void OnEnable()
    {
        if (!toggleAction)
            return;

        toggleAction.action.Enable();
        toggleAction.action.performed += OnTogglePerformed;
    }

    private void OnDisable()
    {
        if (!toggleAction)
            return;

        toggleAction.action.performed -= OnTogglePerformed;
        toggleAction.action.Disable();
    }

    private void OnTogglePerformed(InputAction.CallbackContext ctx)
    {
        SetActive(!IsActive);
    }

    public void SetActive(bool active)
    {
        if (IsActive == active)
            return;

        IsActive = active;

        onGodModeChanged.Invoke(IsActive);

        if (IsActive)
            onGodModeEnabled.Invoke();
        else
            onGodModeDisabled.Invoke();
    }
}