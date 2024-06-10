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

        // Wenn der Spieler-Transform nicht gesetzt ist, suche den Spieler nach dem Tag "Player"
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player").transform;
        }
    }

    void Update()
    {
        //Text mit der aktuellen Höhe des Spielers aktualisieren
        textMeshPro.text = "Height: " + playerTransform.position.y.ToString("F2");
    }
}

