using UnityEngine;

public class Shiftlock : MonoBehaviour
{
    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Toggle cursor lock/unlock when Left Shift is pressed
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // Unlock the cursor and make it visible
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // Lock the cursor to the center and hide it
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
