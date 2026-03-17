using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public static PlayerCam instance;

    [Header("Sensitivity")]
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform modelRotation;

    public bool updatingRotation;

    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!updatingRotation) return;
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        modelRotation.rotation = orientation.rotation;
    }
}
