using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button quitButton;

    [Header("Info Panel")]
    [SerializeField] private GameObject infoPanel;

    [Header("Input")]
    [SerializeField] private InputActionReference closeAction;
    
    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
        infoButton.onClick.AddListener(OnInfoClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        infoPanel.SetActive(false);
    }

    private void OnEnable()
    {
        closeAction.action.Enable();
        closeAction.action.performed += OnClosePerformed;
    }

    private void OnDisable()
    {
        closeAction.action.performed -= OnClosePerformed;
        closeAction.action.Disable();
    }

    private void OnStartClicked()
    {
        SceneLoader.Instance.LoadScene(1);
    }

    private void OnInfoClicked()
    {
        infoPanel.SetActive(true);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnClosePerformed(InputAction.CallbackContext ctx)
    {
        if (infoPanel.activeSelf)
            infoPanel.SetActive(false);
    }
}