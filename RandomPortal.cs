using UnityEngine;

public class RandomPortal : MonoBehaviour
{
    public Transform respawnPoint;
    public Transform level3Target;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the portal");

            // Generiere eine Zufallszahl
            int randomChoice = Random.Range(0, 2); // Gibt 0 oder 1 zurück
            Debug.Log("Random choice: " + randomChoice);

            if (randomChoice == 0)
            {
                // Respawn
                Debug.Log("Respawning player at " + respawnPoint.position);
                TeleportPlayer(other.transform, respawnPoint.position);
            }
            else
            {
                // Teleport zu Level 3
                Debug.Log("Teleporting player to Level 3 at " + level3Target.position);
                TeleportPlayer(other.transform, level3Target.position);
            }
        }
    }

    void TeleportPlayer(Transform playerTransform, Vector3 targetPosition)
    {
        CharacterController characterController = playerTransform.GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false; // Deaktiviere Character Controller vor der Teleportation
        }

        playerTransform.position = targetPosition;
        Debug.Log("Player new position: " + playerTransform.position);

        if (characterController != null)
        {
            characterController.enabled = true; // Reaktiviere Character Controller nach der Teleportation
        }
    }
}
