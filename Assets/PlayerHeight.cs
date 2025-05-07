using UnityEngine;
using Unity.XR.CoreUtils;

public class PlayerHeightAdjuster : MonoBehaviour
{
    public Transform headset; 
    public Transform xrOrigin;
    public float heightOffset;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerStandingHeight"))
        {
            Debug.LogWarning("PlayerHeightAdjuster: No saved height found.");
            return;
        }

        float savedStandingHeight = PlayerPrefs.GetFloat("PlayerStandingHeight");
        float currentHeadsetY = headset.position.y;
        heightOffset = savedStandingHeight - currentHeadsetY;
        PlayerPrefs.SetFloat("PlayerOffset", heightOffset);
        PlayerPrefs.Save();

        // Apply offset to XR Origin so headset aligns with the original standing height
        Vector3 originPosition = xrOrigin.position;
        xrOrigin.position = new Vector3(originPosition.x, originPosition.y + heightOffset, originPosition.z);

        Debug.Log($"PlayerHeightAdjuster: Adjusted XR Origin by {heightOffset} to match saved height.");
    }
}
