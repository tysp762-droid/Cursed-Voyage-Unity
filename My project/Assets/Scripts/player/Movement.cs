using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float gravityMultiplier = 2f;

    [Header("Ground Check")]
    public Transform groundCheck; // optional - position to cast down from
    public float groundCheckDistance = 0.15f;
    public LayerMask groundMask = ~0;

    Rigidbody rb;
    Vector3 moveInput;
    bool jumpRequested;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        // Read player input (frame-based)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveInput = (transform.right * h + transform.forward * v);

        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        // Ground check
        Vector3 origin = groundCheck != null ? groundCheck.position : transform.position;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.01f, groundMask);

        // Horizontal movement (preserve current vertical velocity)
        Vector3 desiredVel = moveInput.normalized * moveSpeed;
        Vector3 currentVel = rb.linearVelocity;
        Vector3 newVel = new Vector3(desiredVel.x, currentVel.y, desiredVel.z);
        rb.linearVelocity = newVel;

        // Jump
        if (jumpRequested && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset vertical
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
        jumpRequested = false;

        // Apply extra gravity for snappier jump/fall
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }
}
