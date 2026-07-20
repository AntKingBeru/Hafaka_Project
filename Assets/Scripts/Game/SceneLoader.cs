using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private Image _fadeImage;
    private Canvas _fadeCanvas;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetUpFadeCanvas();
    }

    private void SetUpFadeCanvas()
    {
        var canvasGo = new GameObject("FadeCanvas");
        canvasGo.transform.SetParent(transform);
        
        _fadeCanvas = canvasGo.AddComponent<Canvas>();
        _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _fadeCanvas.sortingOrder = 999;
        
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();
        
        var imageGo   = new GameObject("FadeImage");
        imageGo.transform.SetParent(canvasGo.transform, false);
        
        _fadeImage = imageGo.AddComponent<Image>();
        _fadeImage.color = Color.clear;
        _fadeImage.raycastTarget = false;

        var rect = imageGo.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    
    public void LoadScene(int buildIndex)
    {
        StartCoroutine(LoadWithFade(buildIndex));
    }

    public void LoadNextScene()
    {
        var next = SceneManager.GetActiveScene().buildIndex + 1;
        LoadScene(next);
    }

    public void LoadMainMenu()
    {
        LoadScene(0);
    }

    private IEnumerator LoadWithFade(int buildIndex)
    {
        yield return StartCoroutine(Fade(0f, 1f));

        Time.timeScale = 1f;
        PauseManager.ClearPause();
        
        var op = SceneManager.LoadSceneAsync(buildIndex);
        op!.allowSceneActivation = false;

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;

        yield return null;
        
        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float from, float to)
    {
        var timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(timer / fadeDuration);
            _fadeImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, t));
            yield return null;
        }

        _fadeImage.color = new Color(0f, 0f, 0f, to);
    }
}