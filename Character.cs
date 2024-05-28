using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    public float walkSpeed = 50f;
    public float runSpeed = 71f;
    public float jumpForce = 80.2f;
    public float gravityScale = 10f;
    public float airControlFactor = 0.5f; // Faktor zur Reduzierung der Bewegung in der Luft
    public float groundCheckDistance = 0.1f; // Abstand zur Überprüfung der Bodenberührung

    private Animator animator;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float gravity = -9.81f;
    private bool isGrounded;
    private bool isJumping;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController-Komponente nicht gefunden! Bitte fügen Sie einen CharacterController hinzu.");
        }
    }

    void Update()
    {
        if (characterController == null)
        {
            return;
        }

        // Manuelle Bodenprüfung mit Raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

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
                moveDirection.y = jumpForce;
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

        // Debug-Ausgaben zum Überprüfen der Zustände
        Debug.Log($"isGrounded: {isGrounded}, isJumping: {isJumping}, velocity.y: {characterController.velocity.y}, currentSpeed: {currentSpeed}");
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
}
