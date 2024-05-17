using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float walkSpeed = 20.0f;
    public float sprintSpeed = 40.0f;
    public float jumpForce = 50.0f;
    public float gravityMultiplier = 10.0f;

    private float currentSpeed;
    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    private Vector3 previousPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        currentSpeed = walkSpeed;
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && moveVertical > 0)
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        // Berechne die aktuelle Geschwindigkeit basierend auf der Bewegung
        float speed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        previousPosition = transform.position;

        // Animator-Parameter setzen
        animator.SetFloat("speed", speed);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // Um Doppelsprünge zu verhindern, setzen wir isGrounded hier auf false.
        }
    }

    void FixedUpdate()
    {
        if (rb.velocity.y < 0)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1), ForceMode.Acceleration);
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1), ForceMode.Acceleration);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Überprüfe, ob der Spieler den Boden oder ein anderes Objekt berührt
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f) // Prüfe, ob die normale der Kollision nach oben zeigt
            {
                isGrounded = true;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Setze isGrounded auf false, wenn der Spieler das Objekt verlässt
        isGrounded = false;
    }
}
