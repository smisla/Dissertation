using UnityEngine;

public class WorldHeightBar : MonoBehaviour
{
    [Header("References")]
    public Transform headset;                 // XR headset (player's head)
    public Transform marker;                  // Marker that moves along bar
    public Transform barStart;                // Left end of the bar (min height)
    public Transform barEnd;                  // Right end of the bar (max height)

    [Header("Height Bounds")]
    public float minHeight;
    public float maxHeight;

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlankMinHeight") && PlayerPrefs.HasKey("PlankMaxHeight"))
        {
            float min = PlayerPrefs.GetFloat("PlankMinHeight");
            float max = PlayerPrefs.GetFloat("PlankMaxHeight");
            SetBounds(min, max);
        }
        else
        {
            Debug.LogWarning("WorldHeightBar: Calibration values not found in PlayerPrefs.");
        }
    }

    private void Update()
    {
        float currentHeight = headset.position.y;

        // Normalize current height between min and max
        float t = Mathf.InverseLerp(minHeight, maxHeight, currentHeight);
        t = Mathf.Clamp01(t);

        // Get position along the bar
        Vector3 targetPos = Vector3.Lerp(barStart.position, barEnd.position, t);
        marker.position = targetPos;

        // Optional: Face camera or stay upright
        //marker.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void SetBounds(float min, float max)
    {
        minHeight = min;
        maxHeight = max;
    }
}

