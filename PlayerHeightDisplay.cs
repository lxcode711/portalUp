using UnityEngine;
using TMPro;

public class PlayerHeightDisplay : MonoBehaviour
{
    public Transform playerTransform; // Referenz auf den Spieler-Transform
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        // Initialisiere TextMeshPro
        textMeshPro = GetComponent<TextMeshProUGUI>();

        // Finde den Spieler, falls die Referenz nicht manuell gesetzt wurde
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }
    }

    void Update()
    {
        // Aktualisiere den Text mit der aktuellen Höhe des Spielers
        textMeshPro.text = "Height: " + playerTransform.position.y.ToString("F2");
    }
}

