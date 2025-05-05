using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using Unity.XR.CoreUtils;

public class PlankDetector : MonoBehaviour
{
    [Header("References")]
    public Transform headset;
    public HandGestureDetector gestureDetector;
    public TextMeshProUGUI plankStatusText;
    public CatSpawner catSpawner;

    [Header("Tuning")]
    public float heightTolerance = 0.05f;

    [Header("Events")]
    public UnityEvent OnPlankBroken;

    private float minHeight;
    private float maxHeight;
    public bool isPlanking = false;
    public bool calibrationLoaded = false;
    public bool previousPlankState = true;

    private void Start()
    {
        LoadCalibration();
    }

    private void Update()
    {

        if (!isPlanking || !calibrationLoaded) return;

        CheckPlankStatus();

        bool isBroken = IsPlankBroken();

        if (isBroken != previousPlankState)
        {
            plankStatusText.text = isBroken ? "Plank Broken" : "Plank Stable";
            previousPlankState = isBroken;

            if (isBroken)
            {
                isPlanking = false;
                HandlePlankBreak();
                OnPlankBroken?.Invoke();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.LogWarning("DEBUG: Triggering simulated plank break");
            isPlanking = false;
            plankStatusText.text = "Plank Broken (Debug)";
            HandlePlankBreak();
            OnPlankBroken?.Invoke();
        }
    }

    public void StartPlankMonitoring()
    {
        if (!calibrationLoaded)
        {
            Debug.LogError("PlankDetector: Calibration data missing. Cannot start monitoring.");
            return;
        }

        isPlanking = true;
        Debug.Log("Plank Monitoring Started.");
    }

    private void LoadCalibration()
    {
        if (!PlayerPrefs.HasKey("PlankMinHeight") || !PlayerPrefs.HasKey("PlankMaxHeight"))
        {
            Debug.LogError("PlankDetector: Calibration values not found. Make sure to calibrate before starting the game.");
            calibrationLoaded = false;
            return;
        }

        minHeight = PlayerPrefs.GetFloat("PlankMinHeight");
        maxHeight = PlayerPrefs.GetFloat("PlankMaxHeight");
        calibrationLoaded = true;

        Debug.Log($"PlankDetector: Loaded calibration - Min: {minHeight}, Max: {maxHeight}");
    }

    private void HandlePlankBreak()
    {
        Debug.Log("PlankDetector: Handling plank break logic.");

        if (catSpawner != null)
        {
            foreach (CatAIController cat in FindObjectsOfType<CatAIController>())
            {
                cat.OnGameStopped();
            }

            foreach (CatBehaviour cat in FindObjectsOfType<CatBehaviour>())
            {
                cat.OnGameStopped();
            }
            catSpawner.StopSpawning();
        }
    }

    private bool IsPlankBroken()
    {
        float currentHeight = headset.position.y;

        string left = gestureDetector.currentLeftGestureName;
        string right = gestureDetector.currentRightGestureName;

        bool heightBroken = currentHeight < (minHeight - heightTolerance) || currentHeight > (maxHeight + heightTolerance);
        bool handsOpen = left == "Open" && right == "Open";

        plankStatusText?.SetText($"Height: {currentHeight:F2}\nLeft: {left}, Right: {right}");

        return heightBroken && handsOpen;
    }
    private void CheckPlankStatus()
    {
        float currentHeight = headset.position.y;

        string left = gestureDetector.currentLeftGestureName;
        string right = gestureDetector.currentRightGestureName;

        bool heightBroken = currentHeight < (minHeight - heightTolerance) || currentHeight > (maxHeight + heightTolerance);
        bool handsOpen = left == "Open" && right == "Open";

        plankStatusText?.SetText($"Height: {currentHeight:F2}\nLeft: {left}, Right: {right}");

        if (heightBroken && handsOpen)
        {
            Debug.Log("PlankDetector: Plank Broken.");
            plankStatusText?.SetText("Plank Broken.");

            isPlanking = false;

            HandlePlankBreak();
            OnPlankBroken?.Invoke();
        }
        else
        {
            plankStatusText?.SetText("Planking...");
        }
    }
}