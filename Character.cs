using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class CharacterControllerScript : MonoBehaviour
{
    public float walkSpeed = 50f;
    public float runSpeed = 71f;
    public float jumpForce = 80.2f;
    public float gravityScale = 10f;
    public float airControlFactor = 0.5f; // Faktor zur Reduzierung der Bewegung in der Luft
    public float groundCheckDistance = 0.3f; // Abstand zur Überprüfung der Bodenberührung
    public Transform startPlatform; // Startplattform
    public float respawnHeightThreshold = -10f; // Höhe für Respawn
    public float jumpBoostDuration = 5.0f; // Dauer des Jump Boosts
    public float jumpBoostMultiplier = 2.0f; // Multiplikator für den Jump Boost
    public TextMeshProUGUI jumpBoostTimerText; // Referenz zum TMP-Text-Element

    private Animator animator;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float gravity = -9.81f;
    private bool isGrounded;
    private bool isJumping;
    private Vector3 respawnPoint; // Respawn Punkt
    private int currentCheckpointIndex = -1;
    private bool isJumpBoosted = false;
    private float jumpBoostEndTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController-Komponente nicht gefunden! Bitte fügen Sie einen CharacterController hinzu.");
        }

        if (startPlatform != null)
        {
            respawnPoint = startPlatform.position; // Setze den Respawn Punkt auf die Startplattform
            Debug.Log($"Startplattform gesetzt auf: {respawnPoint}");
        }
        else
        {
            Debug.LogError("Startplattform nicht gesetzt! Bitte setzen Sie die Startplattform im Inspector.");
        }

        if (jumpBoostTimerText != null)
        {
            jumpBoostTimerText.gameObject.SetActive(false); // Verstecke den Text zunächst
        }
        else
        {
            Debug.LogError("JumpBoostTimerText nicht gesetzt! Bitte setzen Sie das TMP-Text-Element im Inspector.");
        }
    }

    void Update()
    {
        if (characterController == null)
        {
            return;
        }

        // Überprüfe, ob der Spieler unter die Respawn-Höhe gefallen ist
        if (transform.position.y < respawnHeightThreshold)
        {
            Debug.Log("Spieler unter Respawn-Höhe gefallen.");
            GameOver();
        }

        // CharacterController.isGrounded in Kombination mit einem Raycast
        isGrounded = characterController.isGrounded || CheckGrounded();

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 forwardMovement = transform.forward * moveVertical;
        Vector3 rightMovement = transform.right * moveHorizontal;
        Vector3 horizontalMovement = (forwardMovement + rightMovement).normalized * targetSpeed;

        if (isGrounded)
        {
            moveDirection = horizontalMovement;

            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                moveDirection.y = isJumpBoosted ? jumpForce * jumpBoostMultiplier : jumpForce;
                animator.SetBool("IsJumping", true);
                isJumping = true;
            }
            else
            {
                moveDirection.y = 0f;
            }

            // Reset IsFalling when grounded and not jumping
            animator.SetBool("IsFalling", false);
        }
        else
        {
            // Reduziere die Bewegungsgeschwindigkeit in der Luft
            moveDirection.x = horizontalMovement.x * airControlFactor;
            moveDirection.z = horizontalMovement.z * airControlFactor;
            moveDirection.y += gravity * gravityScale * Time.deltaTime;

            // Set IsFalling if in the air and falling
            if (moveDirection.y < 0)
            {
                animator.SetBool("IsFalling", true);
            }
        }

        // Füge eine minimale Bewegung hinzu, um sicherzustellen, dass der CharacterController ständig seine Position überprüft
        if (moveDirection.magnitude < 0.01f && isGrounded)
        {
            moveDirection.y = -0.1f; // Minimal nach unten bewegen, um Bodenkontakt zu behalten
        }

        characterController.Move(moveDirection * Time.deltaTime);

        float currentSpeed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;
        animator.SetFloat("Speed", currentSpeed);
        animator.SetFloat("VerticalSpeed", moveVertical);
        animator.SetBool("IsSprinting", isRunning);

        // Übergangswerte anpassen
        if (currentSpeed > 0 && currentSpeed < walkSpeed)
        {
            animator.SetBool("IsSlowRunning", true);
            animator.SetBool("IsSprinting", false);
        }
        else if (currentSpeed >= 55)
        {
            animator.SetBool("IsSlowRunning", false);
            animator.SetBool("IsSprinting", true);
        }
        else
        {
            animator.SetBool("IsSlowRunning", false);
            animator.SetBool("IsSprinting", false);
        }

        if (isGrounded && !Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("IsJumping", false);
            isJumping = false;
        }

        // Ensure isGrounded is correctly set in Animator
        animator.SetBool("IsGrounded", isGrounded);

        // Aktualisiere den Jump Boost Timer Text
        if (isJumpBoosted && jumpBoostTimerText != null)
        {
            float remainingTime = jumpBoostEndTime - Time.time;
            if (remainingTime > 0)
            {
                jumpBoostTimerText.text = $"Jump Boost: {remainingTime:F1} s";
            }
            else
            {
                jumpBoostTimerText.gameObject.SetActive(false);
                isJumpBoosted = false;
            }
        }

        // Debug-Ausgaben zum Überprüfen der Zustände
        Debug.Log($"isGrounded: {isGrounded}, isJumping: {isJumping}, velocity.y: {characterController.velocity.y}, currentSpeed: {currentSpeed}");
    }

    private bool CheckGrounded()
    {
        // Die Höhe und Länge des Raycast anpassen
        float rayLength = groundCheckDistance;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f; // Raycast leicht über dem Charakter starten

        Debug.DrawRay(rayStart, Vector3.down * rayLength, Color.red);
        return Physics.Raycast(rayStart, Vector3.down, rayLength);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground") && !Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false); // Reset IsFalling when hitting the ground
        }
    }

    private void Respawn()
    {
        Debug.Log("Respawning...");
        characterController.enabled = false; // Deaktivieren Sie den CharacterController vor der Positionsänderung
        transform.position = respawnPoint;
        characterController.enabled = true; // Aktivieren Sie den CharacterController nach der Positionsänderung
    }

    private void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void SetCheckpoint(Vector3 newCheckpoint, int checkpointIndex)
    {
        if (checkpointIndex > currentCheckpointIndex)
        {
            Debug.Log($"Checkpoint erreicht: {checkpointIndex} bei Position {newCheckpoint}");
            respawnPoint = newCheckpoint;
            currentCheckpointIndex = checkpointIndex;
        }
    }

    // Methode zum Aktivieren des Jump Boosts
    public void ActivateJumpBoost()
    {
        if (!isJumpBoosted)
        {
            StartCoroutine(JumpBoostCoroutine());
        }
    }

    private IEnumerator JumpBoostCoroutine()
    {
        isJumpBoosted = true;
        jumpBoostEndTime = Time.time + jumpBoostDuration;
        if (jumpBoostTimerText != null)
        {
            jumpBoostTimerText.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(jumpBoostDuration);
        isJumpBoosted = false;
        if (jumpBoostTimerText != null)
        {
            jumpBoostTimerText.gameObject.SetActive(false);
        }
    }

    // Methode zum Respawn durch Portale
    public void PortalRespawn(Vector3 respawnPosition)
    {
        Debug.Log("Respawning through portal...");
        characterController.enabled = false; // Deaktivieren Sie den CharacterController vor der Positionsänderung
        transform.position = respawnPosition;
        characterController.enabled = true; // Aktivieren Sie den CharacterController nach der Positionsänderung
    }
}
