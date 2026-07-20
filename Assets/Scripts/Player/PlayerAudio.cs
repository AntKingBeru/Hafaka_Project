using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioSource oneShotSource;

    [Header("Clips")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip doubleJumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip victoryClip;
    
    [Header("Walking")]
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private float walkSpeedThreshold = 0.1f;
    [SerializeField] private float walkVolume;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;
    
    [Header("References")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerDash dash;
    [SerializeField] private PlayerGodMode godMode;
    [SerializeField] private PlayerPlatformDetector detector;

    private AudioSource _walkSource;
    private bool _stopped;

    private void Awake()
    {
        oneShotSource.playOnAwake = false;
        oneShotSource.loop = false;
        oneShotSource.spatialBlend = 0f;
        oneShotSource.outputAudioMixerGroup = sfxGroup;
        
        _walkSource = gameObject.AddComponent<AudioSource>();
        _walkSource.clip = walkClip;
        _walkSource.playOnAwake = false;
        _walkSource.loop = true;
        _walkSource.spatialBlend = 0f;
        _walkSource.volume = walkVolume;
        _walkSource.outputAudioMixerGroup = sfxGroup;
    }

    private void Update()
    {
        if (ShouldWalkLoopPlay())
        {
            if (!_walkSource.isPlaying)
                _walkSource.Play();
        }
        else if (_walkSource.isPlaying)
            _walkSource.Stop();
    }

    public void PlayJump() => Play(jumpClip);
    public void PlayDoubleJump() => Play(doubleJumpClip);
    public void PlayLand() => Play(landClip);
    public void PlayDash() => Play(dashClip);
    
    public void PlayDeath()
    {
        StopWalkLoop();
        Play(deathClip);
    }

    public void PlayVictory()
    {
        StopWalkLoop();
        Play(victoryClip);
    }

    private bool ShouldWalkLoopPlay()
    {
        if (_stopped || !walkClip)
            return false;

        if (PauseManager.IsPaused)
            return false;

        if (!detector || !detector.IsGrounded)
            return false;
        
        if (dash && dash.IsDashing)
            return false;

        if (godMode && godMode.IsActive)
            return false;
        
        return movement && Mathf.Abs(movement.HorizontalVelocity) > walkSpeedThreshold;
    }

    private void StopWalkLoop()
    {
        _stopped = true;
        
        if (_walkSource)
            _walkSource.Stop();
    }

    private void Play(AudioClip clip)
    {
        if (!clip)
            return;

        oneShotSource.PlayOneShot(clip, volume);
    }
}