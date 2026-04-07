using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public static PlayerCam instance;

    [Header("Sensitivity")]
    public float sensX = 1f;  // standaardwaarde
    public float sensY = 1f;  // standaardwaarde

    public Transform orientation;
    public Transform modelRotation;

    public bool updatingRotation = true;  // standaard aan

    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        instance = this;
        Debug.Log("PlayerCam Awake");
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("PlayerCam Start - Cursor locked and hidden");
    }

    private void Update()
    {
        if (!updatingRotation)
        {
            Debug.Log("PlayerCam Update skipped - updatingRotation is false");
            return;
        }

        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        modelRotation.rotation = orientation.rotation;

        Debug.Log($"PlayerCam Update - sensX: {sensX}, sensY: {sensY}, mouseX: {mouseX}, mouseY: {mouseY}");
    }

    // Methode om sensitiviteit voor beide assen tegelijk te zetten
    public void SetSensitivity(float newSens)
    {
        sensX = newSens;
        sensY = newSens;
        Debug.Log($"PlayerCam SetSensitivity called - new sensitivity: {newSens}");
    }
}
