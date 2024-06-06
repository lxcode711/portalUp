using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public float targetVolume = 1.0f;
    public float fadeDuration = 5.0f;

    private float currentVolume = 0f;
    private Coroutine fadeCoroutine;

    void Start()
    {
        // Initialisiere die Lautstärke und starte die Musik erst, wenn StartMusic aufgerufen wird.
        if (backgroundMusic != null)
        {
            backgroundMusic.loop = true;
            backgroundMusic.volume = currentVolume;
        }
    }

    public void StartMusic()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
            StartFadeInMusic();
        }
    }

    public void StopMusic()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            StartFadeOutMusic();
        }
    }

    public void StartFadeInMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeInMusic());
    }

    public void StartFadeOutMusic()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutMusic());
    }

    System.Collections.IEnumerator FadeInMusic()
    {
        float startVolume = backgroundMusic.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            backgroundMusic.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        backgroundMusic.volume = targetVolume;
    }

    System.Collections.IEnumerator FadeOutMusic()
    {
        float startVolume = backgroundMusic.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        backgroundMusic.volume = 0f;
        backgroundMusic.Stop();
    }
}
