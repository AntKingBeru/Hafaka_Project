using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Buttons")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button respawnButton;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        winPanel.SetActive(false);
        losePanel.SetActive(false);

        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        respawnButton.onClick.AddListener(OnRespawnClicked);
    }

    public void ShowWin()
    {
        winPanel.SetActive(true);
    }

    public void ShowLose()
    {
        losePanel.SetActive(true);
    }

    private void OnMainMenuClicked()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadMainMenu();
    }

    private void OnRespawnClicked()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}