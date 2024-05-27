using UnityEngine;

public class CharacterControllerScript : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float jumpForce = 20f; // Erhöhte Sprungkraft
    public float gravityScale = 2.3f; // Schwerkraft-Skalierung
    private Animator animator;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float gravity = -9.81f;
    private bool isGrounded;

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

        // Überprüfen, ob der Charakter den Boden berührt
        isGrounded = characterController.isGrounded;

        // Bewegungseingaben erhalten
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce;
                animator.SetBool("IsJumping", true);
            }
            else
            {
                moveDirection.y = 0f;
            }
        }
        else
        {
            moveDirection.x = horizontalMovement.x;
            moveDirection.z = horizontalMovement.z;
            moveDirection.y += gravity * gravityScale * Time.deltaTime;
        }

        // Bewegung anwenden
        characterController.Move(moveDirection * Time.deltaTime);

        // Animator-Parameter setzen
        animator.SetFloat("Speed", new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude);
        animator.SetFloat("VerticalSpeed", moveVertical);
        animator.SetBool("IsSprinting", isRunning);

        // Neue Logik für SlowRun-Animation
        if (moveVertical > 0 && !isRunning && moveHorizontal == 0)
        {
            animator.SetBool("IsSlowRunning", true);
        }
        else
        {
            animator.SetBool("IsSlowRunning", false);
        }

        // Überprüfen, ob der Charakter den Boden berührt und die Sprunganimation beenden
        if (isGrounded && !Input.GetKey(KeyCode.Space))
        {
            animator.SetBool("IsJumping", false);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground") && !Input.GetKey(KeyCode.Space))
        {
            // Falls der Charakter den Boden berührt, setze IsJumping auf false
            animator.SetBool("IsJumping", false);
        }
    }
}
