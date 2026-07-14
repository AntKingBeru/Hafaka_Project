using System.Collections;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem deathSparks;
    [SerializeField] private ParticleSystem winConfetti;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.5f, 0f);
    
    private float _winDuration;

    private void Awake()
    {
        SetUnscaledTime(deathSparks);
        SetUnscaledTime(winConfetti);
        
        _winDuration = winConfetti ? winConfetti.main.duration : 0f;
    }

    public void PlayDeath()
    {
        if (deathSparks)
        {
            deathSparks.transform.position = transform.position + offset;
            deathSparks.Play();
        }
        
        GameUI.Instance.ShowLose();
    }

    public void ShowLoseSilent()
    {
        GameUI.Instance.ShowLose();
    }

    public void PlayWin()
    {
        if (!winConfetti)
        {
            GameUI.Instance.ShowWin();
            return;
        }
        
        winConfetti.transform.position = transform.position + offset;
        winConfetti.Play();
        StartCoroutine(ShowUIAfterDelay(_winDuration, GameUI.Instance.ShowWin));
    }
    
    private IEnumerator ShowUIAfterDelay(float delay, System.Action showUI)
    {
        yield return new WaitForSecondsRealtime(delay);
        showUI();
    }

    private void SetUnscaledTime(ParticleSystem ps)
    {
        if (!ps)
            return;
        var main = ps.main;
        main.useUnscaledTime = true;
    }
}