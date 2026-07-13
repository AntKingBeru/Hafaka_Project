using System.Collections;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem deathSparks;
    [SerializeField] private ParticleSystem winConfetti;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.5f, 0f);

    private void Awake()
    {
        SetUnscaledTime(deathSparks);
        SetUnscaledTime(winConfetti);
    }

    public void PlayDeath()
    {
        if (!deathSparks)
        {
            GameUI.Instance.ShowLose();
            return;
        }
        
        deathSparks.transform.position = transform.position+ offset;
        deathSparks.Play();
        StartCoroutine(ShowUIAfterDelay(
            deathSparks.main.duration,
            GameUI.Instance.ShowLose));
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
        StartCoroutine(ShowUIAfterDelay(
            winConfetti.main.duration,
            GameUI.Instance.ShowWin));
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