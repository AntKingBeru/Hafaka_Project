using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;

    [Header("Input")]
    [SerializeField] private InputActionReference toggleAction;

    public bool IsOpen => panel && panel.activeSelf;

    public System.Func<bool> EscapeConsumer { get; set; }

    private void Awake()
    {
        if (panel)
            panel.SetActive(false);

        if (closeButton)
            closeButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        var sound = SoundManager.Instance;
        if (!sound)
            return;

        masterSlider.SetValueWithoutNotify(sound.GetMasterVolume());
        ambientSlider.SetValueWithoutNotify(sound.GetAmbientVolume());
        sfxSlider.SetValueWithoutNotify(sound.GetSfxVolume());

        masterSlider.onValueChanged.AddListener(sound.SetMasterVolume);
        ambientSlider.onValueChanged.AddListener(sound.SetAmbientVolume);
        sfxSlider.onValueChanged.AddListener(sound.SetSfxVolume);
    }

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
        if (EscapeConsumer != null && EscapeConsumer.Invoke())
            return;

        Toggle();
    }

    public void Toggle()
    {
        if (IsOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (!panel || IsOpen)
            return;

        panel.SetActive(true);
        PauseManager.Instance?.SetPaused(true);
    }

    public void Close()
    {
        if (!panel || !IsOpen)
            return;

        panel.SetActive(false);
        PauseManager.Instance?.SetPaused(false);
    }
}