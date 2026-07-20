using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Info Panel")]
    [SerializeField] private GameObject infoPanel;
    
    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
        infoButton.onClick.AddListener(OnInfoClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        infoPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (SettingsMenu.Instance)
            SettingsMenu.Instance.EscapeConsumer = TryCloseInfoPanel;
    }

    private void OnDisable()
    {
        if (SettingsMenu.Instance)
            SettingsMenu.Instance.EscapeConsumer = null;
    }

    private void OnStartClicked()
    {
        SceneLoader.Instance.LoadScene(1);
    }

    private void OnInfoClicked()
    {
        infoPanel.SetActive(true);
    }

    private void OnSettingsClicked()
    {
        SettingsMenu.Instance?.Open();
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool TryCloseInfoPanel()
    {
        if (!infoPanel.activeSelf)
            return false;

        infoPanel.SetActive(false);
        return true;
    }
}