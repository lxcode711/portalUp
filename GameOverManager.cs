using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayGameOverSound();
    }

    void PlayGameOverSound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    public void RestartGame()
    {
        // Lade die GameScene neu
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Beendet das Spiel
        Application.Quit();
        // Funktioniert nicht im Editor, nur im Build
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
