using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameObject gameOverScreen;
    public AudioClip gameOverMusic;
    private AudioSource audioSource;

    void Start()
    {
        gameOverScreen.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    public void ShowGameOverScreen()
    {
        Debug.Log("Game Over Screen angezeigt");
        gameOverScreen.SetActive(true);
        audioSource.clip = gameOverMusic;
        audioSource.Play();
        Time.timeScale = 0f; // Spielzeit anhalten
        Cursor.lockState = CursorLockMode.None; // Cursor freigeben
        Cursor.visible = true; // Cursor sichtbar machen

        // Hier sicherstellen, dass der Character-Controller keine Eingaben mehr empfängt
        var characterController = FindObjectOfType<CharacterControllerScript>();
        if (characterController != null)
        {
            characterController.DisableInput(); // Eingaben deaktivieren
            characterController.SetGameOverState(true); // Setze Game Over Status
        }
    }

    public void RestartGame()
    {
        Debug.Log("Spiel neu starten");
        Time.timeScale = 1f; // Spielzeit fortsetzen
        Cursor.lockState = CursorLockMode.Locked; // Cursor sperren
        Cursor.visible = false; // Cursor unsichtbar machen

        var characterController = FindObjectOfType<CharacterControllerScript>();
        if (characterController != null)
        {
            characterController.EnableInput(); // Eingaben aktivieren
            characterController.SetGameOverState(false); // Entferne Game Over Status
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Spiel beenden");
        Application.Quit();
    }
}
