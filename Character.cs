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
            respawnPoint = startPlatform.position; // Hier wird der repawn punkt setzet
            
        }
        else
        {
            Debug.LogError("Startplattform nicht gesetzt! Bitte setzen Sie die Startplattform im Inspector.");
        }

        if (jumpBoostTimerText != null)
        {
            jumpBoostTimerText.gameObject.SetActive(false); // Text wird nur angezeigt, wenn der Jump Boost aktiviert ist
        }
        else
        {
            Debug.LogError("JumpBoostTimerText nicht gesetzt! Bitte setzen Sie das TMP-Text-Element im Inspector.");
        }

        // Musik starten
    MusicController musicController = Object.FindFirstObjectByType<MusicController>();
    if (musicController != null)
    {
        musicController.StartMusic();
    }
    }

    void Update()
    {
        if (characterController == null)
        {
            return;
        }

        // Überprüfe, ob der Spieler unter die Respawn-Höhe gefallen ist (-10f)
        if (transform.position.y < respawnHeightThreshold)
        {
            GameOver();
        }

        // CharacterController.isGrounded in Kombination mit einem Raycast zur überprüfung der Bodenberührung
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

            
            animator.SetBool("IsFalling", false);
        }
        else
        {
            // Bewegung in der Luft
            moveDirection.x = horizontalMovement.x * airControlFactor;
            moveDirection.z = horizontalMovement.z * airControlFactor;
            moveDirection.y += gravity * gravityScale * Time.deltaTime;

            
            if (moveDirection.y < 0)
            {
                animator.SetBool("IsFalling", true);
            }
        }

       
        if (moveDirection.magnitude < 0.01f && isGrounded)
        {
            moveDirection.y = -0.1f; // Minimal nach unten um Bodenkontakt zu behalten
        }

        characterController.Move(moveDirection * Time.deltaTime);

        float currentSpeed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;
        animator.SetFloat("Speed", currentSpeed);
        animator.SetFloat("VerticalSpeed", moveVertical);
        animator.SetBool("IsSprinting", isRunning);

        //Animation definitonen
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

        
        animator.SetBool("IsGrounded", isGrounded);

        // Dynamischer Jumpboost-Timer text
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

        
    }

    private bool CheckGrounded()
    {
        //Raycast für die Bodenberührung
        float rayLength = groundCheckDistance;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f; // Raycast leicht über dem char

        Debug.DrawRay(rayStart, Vector3.down * rayLength, Color.red);
        return Physics.Raycast(rayStart, Vector3.down, rayLength);
    }

    // Kollision mit dem Boden
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground") && !Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false); //Zurücksetzen des Fall-Bools
        }
    }

//Respwawn Funktion
    private void Respawn()
    {
        
        characterController.enabled = false; // Deaktivieren Sie den CharacterController vor der Positionsänderung
        transform.position = respawnPoint;
        characterController.enabled = true; // Aktivieren Sie den CharacterController nach der Positionsänderung
    }

//GameOver Funktion
    private void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

 
    // Methode zum Aktivieren des Jump Boosts
    public void ActivateJumpBoost()
    {
        if (!isJumpBoosted)
        {
            StartCoroutine(JumpBoostCoroutine());
        }
    }

    // Coroutine für den Jump Boost

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
     
        characterController.enabled = false;  //Daktivern des CahraacterControllers vor der Positionsänderung
        transform.position = respawnPosition;
        characterController.enabled = true; //Aktivieren des CharacterControllers nach der Positionsänderung
    }
}
