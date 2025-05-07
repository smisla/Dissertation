
using UnityEngine;

public class PoseMonitor : MonoBehaviour
{
    public Transform head;
    public Transform targetArea;
    public float maxOffset = 0.4f;
    public Material overlayMaterial;

    void Update()
    {
        float distance = Vector3.Distance(head.position, targetArea.position);
        float t = Mathf.Clamp01((distance - maxOffset) * 2f);
        overlayMaterial.color = new Color(1, 0, 0, t * 0.5f); // Red fade
    }
}
