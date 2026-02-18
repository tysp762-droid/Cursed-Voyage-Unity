using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float friction = 5f;
    
    private Rigidbody rb;
    private Vector3 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = friction;
    }

    void Update()
    {
        // Get WASD input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveInput = (transform.right * h + transform.forward * v).normalized;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void FixedUpdate()
    {
        // Apply movement
        Vector3 newVel = moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(newVel.x, rb.linearVelocity.y, newVel.z);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position - Vector3.up * 0.5f, Vector3.down, 0.6f, groundLayer);
    }
}
