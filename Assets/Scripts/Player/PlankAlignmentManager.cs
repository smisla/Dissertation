
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlankAlignmentManager : MonoBehaviour
{
    public GameObject ghostAvatar;
    public Transform targetTransform;
    public float alignmentThreshold = 0.2f;
    public float countdownTime = 3f;
    public GameObject countdownUI;

    private bool countdownStarted = false;

    void Start()
    {
        ghostAvatar.SetActive(true);
    }

    void Update()
    {
        float distance = Vector3.Distance(targetTransform.position, ghostAvatar.transform.position);
        if (distance < alignmentThreshold && !countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(StartCountdown());
        }
    }

    System.Collections.IEnumerator StartCountdown()
    {
        countdownUI.SetActive(true);
        yield return new WaitForSeconds(countdownTime);
        ghostAvatar.SetActive(false);
        countdownUI.SetActive(false);
    }
}
