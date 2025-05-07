using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using Unity.XR.CoreUtils;
using TMPro.Examples;

public class PlankDetector : MonoBehaviour
{
    [Header("References")]
    public Transform headset;
    public Transform XROrigin;
    public HandGestureDetector gestureDetector;
    public TextMeshProUGUI plankStatusText;
    public TextMeshProUGUI detectorText;
    public CatSpawner catSpawner;
    public CountdownTimer countdown;
    public PostProcessingController postFXController;
    public GameObject cameraOffset;
    public float playerHeightOffset;

    [Header("Tuning")]
    public float heightTolerance = 0.5f;

    [Header("Events")]
    public UnityEvent OnPlankBroken;
    private bool plankBreakInitiated = false;
    private float plankBreakTimer = 0f;
    private float breakCountdownDuration = 3f;

    [Header("Effects")]
    public GameObject postProcessingEffects;
    public AudioSource heartbeatAudio;
    //script that has game end handling

    [Header("Height Logic")]
    private float minHeight;
    private float maxHeight;
    public float currentHeight;
    public bool isPlanking = false;
    public bool calibrationLoaded = false;
    public float adjustedHeight;
    public bool previousPlankState = true;
    private LayerMask groundLayer;
    //public Vector3 fixedPos = new Vector3(0, 0, 0);
    public bool lockRotation = true;
    public bool gotHeight = false;

    private void Start() // Loads player boundaries
    {
        
        LoadCalibration();

        playerHeightOffset = PlayerPrefs.GetFloat("PlayerStandingHeight");

        
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void LateUpdate()
    {
        //if (!gotHeight)
        //{
        //    currentHeight = currentHeight + playerHeightOffset;
        //    Debug.Log($"Height: {adjustedHeight}");
        //    gotHeight = true;
        //}

        //XROrigin.transform.position = fixedPos;
    }

    private void Update() // if isPlanking and calibration loaded is true, run detection logic. Checks for plank break, handles plank break.
    {
        TrackHeight();
        //float currentHeight = headset.position.y;
        string left = gestureDetector.currentLeftGestureName;
        string right = gestureDetector.currentRightGestureName;

        bool heightBroken = currentHeight < (minHeight - heightTolerance) || currentHeight > (maxHeight + heightTolerance);
        bool handsOpen = left == "Open" && right == "Open";

        detectorText?.SetText($"MinHeight: {minHeight}, MaxHeight: {maxHeight}, Height: {currentHeight:F2}\nLeft: {left}, Right: {right}");

        if (!isPlanking || !calibrationLoaded) return;

        if (countdown.countdownComplete)
        {

            if (heightBroken && handsOpen)
            {
                // Immediate Plank Break
                HandleImmediateBreak();
            }
            else if (heightBroken && !handsOpen)
            {
                // Player Out of Bounds
                if (!plankBreakInitiated)
                {
                    // Start Countdown
                    plankBreakInitiated = true;
                    plankBreakTimer = 0f;
                    Debug.Log("PlankDetector: Countdown started due to height break");
                }
                plankBreakTimer += Time.deltaTime;
                float t = plankBreakTimer / breakCountdownDuration;
                UpdateBreakCountdownEffects(t);
                //StartBreakCountdownEffects();
                    

                //plankBreakTimer += Time.deltaTime;
                //UpdateBreakCountdownEffects(plankBreakTimer / breakCountdownDuration);

                if (plankBreakTimer >= breakCountdownDuration)
                {
                    HandleCountdownBreak();
                }
            }
            else
            {
                // Player in bounds
                if (plankBreakInitiated)
                {
                    plankBreakInitiated = false;
                    plankBreakTimer = 0f;
                    float t = 1.5f;
                    StopBreakCountdownEffects(t);
                    Debug.Log("PlankDetector: Returned to valid plank position. Countdown cancelled");
                }

                if (handsOpen)
                {
                    plankStatusText?.SetText("Hands not supporting plank!");
                    //visual warning
                }
                else
                {
                    plankStatusText?.SetText("Planking...");
                }
            }
        }
        

        //CheckPlankStatus();

        //bool isBroken = IsPlankBroken();

        //if (isBroken != previousPlankState)
        //{
        //    plankStatusText.text = isBroken ? "Plank Broken" : "Plank Stable";
        //    previousPlankState = isBroken;

        //    if (isBroken)
        //    {
        //        isPlanking = false;
        //        HandlePlankBreak();
        //        OnPlankBroken?.Invoke();
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    Debug.LogWarning("DEBUG: Triggering simulated plank break");
        //    isPlanking = false;
        //    plankStatusText.text = "Plank Broken (Debug)";
        //    HandlePlankBreak();
        //    OnPlankBroken?.Invoke();
        //}
    }

    private void TrackHeight()
    {
        RaycastHit hit;

        if (Physics.Raycast(headset.transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            Debug.DrawRay(headset.transform.position, Vector3.down * 20, Color.red);
            currentHeight = hit.distance;
        }
        else
        {
            // If the raycast doesn't hit the ground, log an error
            Debug.LogError("Raycast did not hit the ground.");
        }
    }

        private void HandleImmediateBreak()
    {
        Debug.Log("PlankDetector: Instant plank break triggered.");

        isPlanking = false;
        plankBreakInitiated = false;
        float t = 1.5f;
        StopBreakCountdownEffects(t);
        HandlePlankBreak();

        Debug.LogWarning("GAME ENDED, GOING TO NEXT SCENE");
        // end game
        //if (gameEndHandler.IsCatOnPlank())
        //    gameEndHandler.MakeCatFall();
        //else
        //    gameEndHandler.FadeToWhiteAndEnd();

        OnPlankBroken?.Invoke();
    }

    private void HandleCountdownBreak()
    {
        Debug.Log("PlankDetector: Plank broke after countdown.");

        isPlanking = false;
        plankBreakInitiated = false;
        plankBreakTimer = 0f;

        HandlePlankBreak();

        //if (gameEndHandler.IsCatOnPlank())
        //    gameEndHandler.MakeCatFall();
        //else
        //    gameEndHandler.FadeToWhiteAndEnd();

        OnPlankBroken?.Invoke();
    }

    //private void StartBreakCountdownEffects()
    //{
    //    if (postProcessingEffects != null)
    //    {
    //        postProcessingEffects.SetActive(true);
    //    }

    //    if (heartbeatAudio != null)
    //    {
    //        heartbeatAudio.Play();
    //    }
    //}

    private void UpdateBreakCountdownEffects(float t)
    {
            t = Mathf.Clamp01(t);
            postFXController?.UpdateEffectProgress(t);
            heartbeatAudio.volume = Mathf.Lerp(0f, 1f, t);
    }

    private IEnumerator LerpResetEffects(float duration)
    {
        float time = 0f;

        // Assume postFXController keeps track of current effect strength internally
        float startVolume = heartbeatAudio.volume;
        float startPostProgress = postFXController.CurrentProgress;

        while (time < duration)
        {
            float lerpT = time / duration;

            heartbeatAudio.volume = Mathf.Lerp(startVolume, 0f, lerpT);
            postFXController?.UpdateEffectProgress(Mathf.Lerp(startPostProgress, 0f, lerpT));

            time += Time.deltaTime;
            yield return null;
        }

        // Finalize
        heartbeatAudio.volume = 0f;
        postFXController?.ResetEffects();
    }
    private void StopBreakCountdownEffects(float t)
    {
        StopCoroutine(LerpResetEffects(1f));

        StartCoroutine(LerpResetEffects(t / 2f));
        //postFXController?.ResetEffects();
        //plankBreakTimer += Time.deltaTime;
        //float tr = 1f;

        //postFXController?.ResetEffects(tr);
        //heartbeatAudio.volume = Mathf.Lerp(0f, 1f, tr); // Optional
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

    public void HandlePlankBreak()
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

        if ( handsOpen )
        {
            Debug.Log("PlankDetector: Plank Broken.");
            plankStatusText?.SetText("Plank HANDS Broken.");

            isPlanking = false;

            HandlePlankBreak();
            OnPlankBroken?.Invoke();
        }
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