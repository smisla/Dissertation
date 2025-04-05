using UnityEngine;

public class MenuPositioning : MonoBehaviour
{
    public Transform playerCamera; // Reference to the player's camera (VR camera)
    public float distanceFromPlayer = 4f; // How far the menu is from the player
    public float heightOffset = 0f; // Height offset for the menu (adjust based on the desired height)
    public float lockTime = 0.1f; // Time after which the menu locks its position in seconds

    private float timer = 0f; // Timer to track time passed
    private bool isLocked = false; // Flag to check if the menu position is locked

    void Update()
    {
        if (!isLocked)
        {
            timer += Time.deltaTime; // Increase the timer by the time passed since the last frame

            // If the timer exceeds the lock time, lock the position
            if (timer >= lockTime)
            {
                isLocked = true;
            }

            // Calculate the position in front of the player (relative to the camera)
            Vector3 targetPosition = playerCamera.position + playerCamera.forward * distanceFromPlayer;
            targetPosition.y = playerCamera.position.y + heightOffset; // Maintain height relative to the player

            // Set the menu's position to the target position
            transform.position = targetPosition;

            // Make the menu always face the camera
            transform.LookAt(playerCamera);
            transform.Rotate(0, 180, 0); // Optional, to ensure the front of the menu faces the player properly
        }
    }
}


