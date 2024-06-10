using UnityEngine;

public class PortalJumpAndRespawn : MonoBehaviour
{
    public bool isJumpBoostPortal; // Gibt an, ob dieses Portal einen Jump Boost gewährt
    public GameObject respawnPoint; // GameObject, das den Respawn-Punkt darstellt

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterControllerScript playerScript = other.GetComponent<CharacterControllerScript>();
            if (playerScript != null)
            {
                if (isJumpBoostPortal) // Überprüfen, ob es sich um ein Jump-Boost-Portal handelt
                {
                    Debug.Log("Jump Boost aktiviert");
                    playerScript.ActivateJumpBoost();
                }
                else
                {
                    if (respawnPoint != null)
                    {
                        Debug.Log("Respawn an " + respawnPoint.transform.position);
                        playerScript.PortalRespawn(respawnPoint.transform.position);
                    }
                    else
                    {
                        Debug.LogError("RespawnPoint ist nicht gesetzt!");
                    }
                }
            }
        }
    }
}
