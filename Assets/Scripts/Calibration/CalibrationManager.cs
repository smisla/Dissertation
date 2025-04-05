using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CalibrationManager : MonoBehaviour
{
    public TextMeshProUGUI calibrationText;
    public float countdownTime = 5f;

    private XRNode inputSource = XRNode.Head;
    private float minHeight = float.PositiveInfinity;
    private float maxHeight = float.NegativeInfinity;
    private bool isCalibratingDown = false;
    private bool isCalibratingUp = false;
    public Transform uiCenter;

    public float yHeight = 0.5f;
    public Camera playerCamera;
    public Canvas calibrationCanvas;
    private LayerMask groundLayer;

    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogError("Main camera not found. Ensure the camera is tagged as MainCamera.");
        }
        Vector3 newCanvasPosition = uiCenter.position + uiCenter.forward * 1.5f;  // 1.5 meters in front of UICenter
        newCanvasPosition.y = uiCenter.position.y - 0.3f;  // Lower it slightly (adjust as needed)
        calibrationCanvas.transform.position = newCanvasPosition;

        // Make sure the canvas always faces the player (keeping the same rotation as the camera)
        calibrationCanvas.transform.rotation = uiCenter.rotation;

        groundLayer = LayerMask.GetMask("Ground");

        StartCalibration();
    }

    private void Update()
    {
        TrackHeadPosition();
        Vector3 newCanvasPosition = uiCenter.position + uiCenter.forward * 1.5f;  // 1.5 meters in front of UICenter
        newCanvasPosition.y = uiCenter.position.y - 0.3f;  // Lower it slightly (adjust as needed)
        calibrationCanvas.transform.position = newCanvasPosition;

        // Make sure the canvas always faces the player (keeping the same rotation as the camera)
        calibrationCanvas.transform.rotation = uiCenter.rotation;

    }
    private void StartCalibration()
    {
        calibrationText.text = "Get into plank position and look down.";
        StartCoroutine(CountdownDown());
    }

    private IEnumerator CountdownDown()
    {
        yield return new WaitForSeconds(5f);
        isCalibratingDown = true;
        float timer = countdownTime;
        while (timer > 0f)
        {
            calibrationText.text = $"Looking down... {timer: 0} seconds left.";
            timer -= Time.deltaTime;
            yield return null;
        }

        calibrationText.text = "Looking up... please tilt your head back.";
        StartCoroutine(CountdownUp());
    }


    private IEnumerator CountdownUp()
    {
        isCalibratingDown = false;
        yield return new WaitForSeconds(5f);
        isCalibratingUp = true;
        float timer = countdownTime;
        while (timer > 0f)
        {
            calibrationText.text = $"Looking up... {timer:0} seconds left";
            timer -= Time.deltaTime;
            yield return null;
        }

        calibrationText.text = "Calibration complete!";
        if (minHeight != float.PositiveInfinity || maxHeight != float.NegativeInfinity)
        {
            EndCalibration();
        }
    }

    private void TrackHeadPosition()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            Debug.DrawRay(playerCamera.transform.position, Vector3.down * 10, Color.red);
            float distanceToGround = hit.distance;
            Debug.Log($"Ray hit the ground at distance: {distanceToGround}");
            if (isCalibratingDown)
            {
                minHeight = Mathf.Min(minHeight, distanceToGround);
                Debug.Log($"New minHeight: {minHeight}");

            }
            else if (isCalibratingUp)
            {
                maxHeight = Mathf.Max(maxHeight, distanceToGround);
                Debug.Log($"New maxHeight: {maxHeight}");
            }
            else
            {
                // If the raycast doesn't hit the ground, log an error
                Debug.LogError("Raycast did not hit the ground.");
            }
        }
        
    }
    private void EndCalibration()
    {
        // Store the minHeight and maxHeight values for later use (e.g., save to a global manager)
        Debug.Log($"Min Height: {minHeight}, Max Height: {maxHeight}");

        SceneManager.LoadScene("StartScene");
        

        // Now, pass this data to the main game or a global game manager
    }
}
