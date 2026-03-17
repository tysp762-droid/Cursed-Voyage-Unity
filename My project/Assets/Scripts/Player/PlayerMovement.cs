using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float runningSpeed;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;

    [Header("Grounded Variables")]
    [SerializeField] private float groundedCheckOffset;
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;

    [Header("Serialized References")]
    [SerializeField] private Transform Orientation;

    //Private Variables
    private bool grounded;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;
    private bool readyToJump = true;

    private enum MoveState { walking, running, jumping }
    private MoveState moveState = MoveState.walking;

    //Private References
    private Rigidbody rb;
    private CapsuleCollider playerCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        Inputs();
        SetGrounded();
    }

    void FixedUpdate()
    {
        SetGroundDrag();
        Move();
        SpeedControl();
    }

    private void SetGroundDrag()
    {
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void SetGrounded()
    {
        grounded = Physics.Raycast(transform.position + new Vector3(groundedCheckOffset, 0f, groundedCheckOffset), Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer) ||
            Physics.Raycast(transform.position + new Vector3(-groundedCheckOffset, 0f, groundedCheckOffset), Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer) ||
            Physics.Raycast(transform.position + new Vector3(groundedCheckOffset, 0f, -groundedCheckOffset), Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer) ||
            Physics.Raycast(transform.position + new Vector3(-groundedCheckOffset, 0f, -groundedCheckOffset), Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
    }

    private void SpeedControl()
    {
        float maxSpeed = 0;
        if (moveState == MoveState.walking)
            maxSpeed = moveSpeed;
        else if (moveState == MoveState.running)
            maxSpeed = runningSpeed;
        
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Move()
    {
        rb.useGravity = true;
        moveDirection = Orientation.forward * verticalInput + Orientation.right * horizontalInput;

        if (grounded)
            if (moveState == MoveState.walking)
                rb.AddForce(moveDirection * moveSpeed * 50f, ForceMode.Force);
            else if (moveState == MoveState.running)
                rb.AddForce(moveDirection * runningSpeed * 50f, ForceMode.Force);
        
        else if (!grounded)
            rb.AddForce(moveDirection * moveSpeed * 50f * airMultiplier, ForceMode.Force);
    }

    private void Inputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space) && grounded && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(KeyCode.LeftShift) && grounded)
            moveState = MoveState.running;
        else if (!Input.GetKey(KeyCode.LeftShift) && grounded)
            moveState = MoveState.walking;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
