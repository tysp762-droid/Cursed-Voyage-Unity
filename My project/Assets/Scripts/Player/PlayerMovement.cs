using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float runningSpeed;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float airMultiplier;

    [Header("Grounded Variables")]
    [SerializeField] private float groundedCheckOffset;
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;

    [Header("Stamina Variables")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f;       // per second while running
    [SerializeField] private float staminaRegenRate = 15f;       // per second
    [SerializeField] private float staminaRegenCooldown = 2f;    // delay before regen starts

    [Header("UI (Optional)")]
    [Tooltip("Assign a TextMeshProUGUI in your Canvas to show stamina, e.g. 'Stamina 87%'")]
    [SerializeField] private TextMeshProUGUI staminaText;

    [Header("Serialized References")]
    [SerializeField] private Transform Orientation;

    // Private Variables
    private bool grounded;
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;
    private bool readyToJump = true;

    private float currentStamina;
    private float staminaRegenTimer = 0f;

    private enum MoveState { walking, running, jumping }
    private MoveState moveState = MoveState.walking;

    // Private References
    private Rigidbody rb;
    private CapsuleCollider playerCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerCollider = GetComponent<CapsuleCollider>();

        currentStamina = maxStamina;
    }

    void Update()
    {
        Inputs();
        SetGrounded();
        HandleStamina(Time.deltaTime);

        // If you assigned the TMP text in the inspector, this keeps it updated.
        UpdateStaminaText();
    }

    void FixedUpdate()
    {
        SetGroundDrag();
        Move();
        SpeedControl();
    }

    private void SetGroundDrag()
    {
        rb.linearDamping = grounded ? groundDrag : 0f;
    }

    private void SetGrounded()
    {
        grounded =
            Physics.Raycast(transform.position + new Vector3(groundedCheckOffset, 0f, groundedCheckOffset), Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer) ||
            Physics.Raycast(transform.position + new Vector3(-groundedCheckOffset, 0f, groundedCheckOffset), Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer) ||
            Physics.Raycast(transform.position + new Vector3(groundedCheckOffset, 0f, -groundedCheckOffset), Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer) ||
            Physics.Raycast(transform.position + new Vector3(-groundedCheckOffset, 0f, -groundedCheckOffset), Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
    }

    private void SpeedControl()
    {
        float maxSpeed = (moveState == MoveState.running) ? runningSpeed : moveSpeed;

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
        {
            float speed = (moveState == MoveState.running) ? runningSpeed : moveSpeed;
            rb.AddForce(moveDirection * speed * 50f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection * moveSpeed * 50f * airMultiplier, ForceMode.Force);
        }
    }

    private void Inputs()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // Jump
        if (Input.GetKey(KeyCode.Space) && grounded && readyToJump)
        {
            readyToJump = false;
            Jump();
        }

        // Run (requires stamina)
        if (Input.GetKey(KeyCode.LeftShift) && grounded && currentStamina > 0f)
        {
            moveState = MoveState.running;
            staminaRegenTimer = 0f; // reset regen delay while running
        }
        else if (grounded)
        {
            moveState = MoveState.walking;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            readyToJump = true;
        }
    }

    private void HandleStamina(float deltaTime)
    {
        bool isTryingToMove = (horizontalInput != 0f || verticalInput != 0f);

        if (moveState == MoveState.running && isTryingToMove)
        {
            currentStamina -= staminaDrainRate * deltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                moveState = MoveState.walking; // force stop running
            }
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                staminaRegenTimer += deltaTime;
                if (staminaRegenTimer >= staminaRegenCooldown)
                {
                    currentStamina += staminaRegenRate * deltaTime;
                    if (currentStamina > maxStamina) currentStamina = maxStamina;
                }
            }
            else
            {
                staminaRegenTimer = 0f;
            }
        }
    }

    // --- UI API ---

    /// <summary>
    /// Assign the TMP text from code (optional). Example:
    /// playerMovement.SetStaminaText(myTmpText);
    /// </summary>
    public void SetStaminaText(TextMeshProUGUI tmp)
    {
        staminaText = tmp;
        UpdateStaminaText(true);
    }

    /// <summary>
    /// Updates the stamina TMP text to: "Stamina 87%".
    /// Called automatically in Update() if staminaText is assigned.
    /// </summary>
    public void UpdateStaminaText(bool force = false)
    {
        if (staminaText == null) return;

        // If you want to reduce string updates, you can gate by a small threshold.
        // "force" is useful right after assigning the text.
        int percent = Mathf.RoundToInt(GetStaminaPercent() * 100f);
        staminaText.text = $"Stamina {percent}%";
    }

    public float GetStaminaPercent()
    {
        return (maxStamina <= 0f) ? 0f : (currentStamina / maxStamina);
    }
}
