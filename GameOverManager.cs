using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private AudioSource audioSource; // Referenz zum AudioSource

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayGameOverSound();

        // Cursor sichtbar machen und entsperren
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        // Laden der GameScene neu
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Beendet das Spiel
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
