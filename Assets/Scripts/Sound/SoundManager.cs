using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private const string MasterParam = "MasterVolume";
    private const string AmbientParam = "AmbientVolume";
    private const string SfxParam = "SFXVolume";

    private const string MasterKey = "vol_master";
    private const string AmbientKey = "vol_ambient";
    private const string SfxKey = "vol_sfx";

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup ambientGroup;

    [Header("Music")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private AudioSource musicSource;
    [Tooltip("Build index of the main menu scene.")]
    [SerializeField] private int mainMenuSceneIndex;
    
    [Header("Pause Ducking")]
    [Range(0f, 1f)]
    [SerializeField] private float pausedAmbientScale = 0.4f;
    
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;
        musicSource.outputAudioMixerGroup = ambientGroup;
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Start()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MasterKey, 1f));
        SetAmbientVolume(PlayerPrefs.GetFloat(AmbientKey, 1f));
        SetSfxVolume(PlayerPrefs.GetFloat(SfxKey, 1f));
        
        UpdateMusicForScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void PlayMenuMusic() => PlayMusic(menuMusic);
    public void PlayLevelMusic() => PlayMusic(levelMusic);
    
    public void PlayMusic(AudioClip clip)
    {
        if (!clip)
            return;
        
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.Play();
        musicSource.time = 0.0294f;
    }

    public void StopMusic() => musicSource.Stop();
    
    public float GetMasterVolume() => PlayerPrefs.GetFloat(MasterKey, 1f);
    public float GetAmbientVolume() => PlayerPrefs.GetFloat(AmbientKey, 1f);
    public float GetSfxVolume() => PlayerPrefs.GetFloat(SfxKey, 1f);

    public void SetMasterVolume(float value) => Apply(MasterParam, MasterKey, value);
    public void SetAmbientVolume(float value) => Apply(AmbientParam, AmbientKey, value);
    public void SetSfxVolume(float value) => Apply(SfxParam, SfxKey, value);
    
    public void SetPausedDucking(bool paused)
    {
        var target = GetAmbientVolume() * (paused ? pausedAmbientScale : 1f);

        if (!mixer)
            return;

        var db = target <= 0.0001f ? -80f : Mathf.Log10(target) * 20f;
        mixer.SetFloat(AmbientParam, db);
    }

    private void Apply(string param, string prefsKey, float linear)
    {
        linear = Mathf.Clamp01(linear);

        PlayerPrefs.SetFloat(prefsKey, linear);

        if (!mixer)
            return;
        
        var db = linear <= 0.0001f ? -80f : Mathf.Log10(linear) * 20f;
        mixer.SetFloat(param, db);
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMusicForScene(scene.buildIndex);
    }

    private void UpdateMusicForScene(int buildIndex)
    {
        PlayMusic(buildIndex == mainMenuSceneIndex ? menuMusic : levelMusic);
    }
}