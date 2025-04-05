using UnityEngine;

public class LockPlayerRotation : MonoBehaviour
{
    [Header("XR Origin")]
    public Transform xrOrigin;  // Reference to the XR Origin (or player's parent object)

    [Header("Initial Rotation")]
    public float initialYRotation = 0f;  // Set this to the desired starting rotation around the Y-axis

    void Start()
    {
        // Lock the player's rotation around the Y-axis on initialization
        LockRotation();
    }

    void Update()
    {
        // Keep the rotation locked on the Y-axis
        LockRotation();
    }

    // Locks the player's rotation around the Y-axis
    private void LockRotation()
    {
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin not assigned.");
            return;
        }

        // Get the current rotation of the player
        Vector3 currentRotation = xrOrigin.rotation.eulerAngles;

        // Lock the Y rotation while preserving the X and Z rotations
        currentRotation.y = initialYRotation;

        // Set the rotation back to the XR Origin
        xrOrigin.rotation = Quaternion.Euler(currentRotation);
    }
}
