using UnityEngine;

public class FirstPersonCam : MonoBehaviour
{
    public float mouseSensitivity = 2f;
    public Transform playerBody;

    private float verticalRotation = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
                Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Verticale rotatie bijhouden en beperken
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Camera verticaal draaien met Vector3
        transform.localEulerAngles = new Vector3(verticalRotation, 0, 0);

        // Speler horizontaal draaien met Vector3
        playerBody.Rotate(new Vector3(0, mouseX, 0));
    }
}
