using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player; // Dein Player GameObject
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    private float xRotation = 0f;

    void Start()
    {
        // Versteckt und sperrt den Cursor in der Mitte des Bildschirms
        Cursor.lockState = CursorLockMode.Locked;

        // Überprüfen, ob die Referenzen korrekt gesetzt sind
        if (player == null)
        {
            Debug.LogError("Player-Transform ist nicht zugewiesen!");
        }

        if (cameraTransform == null)
        {
            Debug.LogError("Camera-Transform ist nicht zugewiesen!");
        }
    }

    void Update()
    {
        // Überprüfen, ob die Referenzen korrekt sind
        if (player == null || cameraTransform == null)
        {
            Debug.LogError("Referenzen sind nicht korrekt gesetzt!");
            return; // Beende das Update, wenn die Referenzen nicht gesetzt sind
        }

        // Maus Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Drehung des Spielers und der Kamera
        player.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
