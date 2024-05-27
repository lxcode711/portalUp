using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    public float walkSpeed = 20f;
    public float runSpeed = 30f;
    public float jumpForce = 40f;
    public float gravityScale = 4.5f;

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

        isGrounded = characterController.isGrounded;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 forwardMovement = transform.forward * moveVertical;
        Vector3 rightMovement = transform.right * moveHorizontal;
        Vector3 horizontalMovement = (forwardMovement + rightMovement).normalized * speed;

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
            moveDirection.x = horizontalMovement.x;
            moveDirection.z = horizontalMovement.z;
            moveDirection.y += gravity * gravityScale * Time.deltaTime;

            // Set IsFalling if in the air and falling
            if (moveDirection.y < 0)
            {
                animator.SetBool("IsFalling", true);
            }
        }

        characterController.Move(moveDirection * Time.deltaTime);

        animator.SetFloat("Speed", new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude);
        animator.SetFloat("VerticalSpeed", moveVertical);
        animator.SetBool("IsSprinting", isRunning);

        if (moveVertical > 0 && !isRunning && moveHorizontal == 0)
        {
            animator.SetBool("IsSlowRunning", true);
        }
        else
        {
            animator.SetBool("IsSlowRunning", false);
        }

        if (isGrounded && !Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("IsJumping", false);
            isJumping = false;
        }

        // Ensure isGrounded is correctly set in Animator
        animator.SetBool("IsGrounded", isGrounded);

        // Debug-Ausgaben zum Überprüfen der Zustände
        Debug.Log($"isGrounded: {isGrounded}, isJumping: {isJumping}, velocity.y: {characterController.velocity.y}");
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
