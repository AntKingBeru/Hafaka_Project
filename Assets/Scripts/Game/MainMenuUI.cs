using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button quitButton;

    [Header("Info Panel")]
    [SerializeField] private GameObject infoPanel;
    
    [SerializeField] private SettingsMenu settingsMenu;
    
    private void Awake()
    {
        startButton.onClick.AddListener(OnStartClicked);
        infoButton.onClick.AddListener(OnInfoClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        infoPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (settingsMenu)
            settingsMenu.EscapeConsumer = TryCloseInfoPanel;
    }

    private void OnDisable()
    {
        if (settingsMenu)
            settingsMenu.EscapeConsumer = null;
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

    private bool TryCloseInfoPanel()
    {
        if (infoPanel.activeSelf)
            return false;

        infoPanel.SetActive(false);
        return true;
    }
}