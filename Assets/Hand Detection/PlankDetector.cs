using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using Unity.XR.CoreUtils;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;
    public UnityEvent onRecognised;

}

public class PlankDetector : MonoBehaviour
{
    // Provides access to the hand tracking system
    public TextMeshProUGUI gestureText;

    public float threshold = 0.1f;
    public XRHandSubsystem handSubsystem;
    public List<Gesture> gestures;
    public bool debugMode = true;
    private Gesture previousGesture;


    private Gesture previousLeftGesture;
    private Gesture previousRightGesture;

    private string leftHandGesture = "None";
    private string rightHandGesture = "None";

    //Height Calibration Variables

    private float minHeight;
    private float maxHeight;
    public float heightTolerance = 0.2f;

    private bool isPlanking = false;


    private void Start()
    {
        //Get the active XR hand tracking subsystem
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();

        previousLeftGesture = new Gesture();
        previousRightGesture = new Gesture();  
        // previousGesture = new Gesture();
        if (handSubsystem == null)
        {
            Debug.LogError("XRHandSubsystem not found! Ensure XR Hands package is installed and enabled.");
        }
        else
        {
            Debug.Log("XRHandSubsystem successfully loaded.");
        }
    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.L))
        {
            SaveGesture(handSubsystem.leftHand, leftHandGesture, "Left Hand");
            Debug.Log("Saving LEFT HAND Gesture...");
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            SaveGesture(handSubsystem.rightHand, rightHandGesture, "Right Hand");
            Debug.Log("Saving RIGHT HAND Gestyre...");
        }

        Gesture currentGesture = Recognise();
        bool hasRecognised = !currentGesture.Equals(new Gesture());

        if (hasRecognised && currentGesture.Equals(previousGesture))
        {
            Debug.Log("New Gesture Found: " + currentGesture.name);
            previousGesture = currentGesture;
            currentGesture.onRecognised.Invoke();
        }

        if (gestureText != null)
        {
            gestureText.text = "Gesture: " + currentGesture.name;
        }

        else
        {
            if (gestureText != null)
            {
                gestureText.text = "Gesture: None";
            }
        }
    }

    void SaveGesture()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();

        XRHand leftHand = handSubsystem.leftHand;
        XRHand rightHand = handSubsystem.rightHand;

        if (leftHand.isTracked)
        {
            Debug.Log("Left hand detected. Saving...");
            SaveHandData(leftHand, data);
        }
        else if (rightHand.isTracked)
        {
            Debug.Log("Right hand detected. Saving...");
            SaveHandData(rightHand, data);
        }

        if (!leftHand.isTracked && !rightHand.isTracked)
        {
            Debug.LogError("No hand detected when trying to save!");
            return;
        }

        g.fingerData = data;
        gestures.Add(g);
        Debug.Log("Gesture saved successfully! Recorded " + g.fingerData.Count + " joint positions.");
    }

    private void SaveHandData(XRHand hand, List<Vector3> data)
    {
        XRHandJoint wristJoint = hand.GetJoint(XRHandJointID.Wrist);

        if (!wristJoint.TryGetPose(out Pose wristPose))
        {
            Debug.LogError("Failed to get wrist pose.");
        }

        foreach (XRHandJointID jointID in new XRHandJointID[]
        {
            XRHandJointID.ThumbTip, XRHandJointID.IndexTip, XRHandJointID.MiddleTip,
            XRHandJointID.RingTip, XRHandJointID.LittleTip,
            XRHandJointID.IndexIntermediate, XRHandJointID.MiddleIntermediate,
            XRHandJointID.RingIntermediate, XRHandJointID.LittleIntermediate
        })
        {
            XRHandJoint joint = hand.GetJoint(jointID);
            if (joint.TryGetPose(out Pose pose))
            {
                Vector3 relativePosition = pose.position - wristPose.position;
                data.Add(relativePosition);
                Debug.Log($"Saved {jointID} relative to wrist: {relativePosition}");
            }
        }
    }

    Gesture Recognise()
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;


        XRHand leftHand = handSubsystem.leftHand;
        XRHand rightHand = handSubsystem.rightHand;

        XRHand hand = leftHand.isTracked ? leftHand : rightHand;

        if (hand == null || !hand.isTracked)
        {
            Debug.LogWarning("No valid hand detected");
            return currentGesture;
        }

        XRHandJoint wristJoint = hand.GetJoint(XRHandJointID.Wrist);
        Pose wristPose = new Pose();

        if (!wristJoint.TryGetPose(out wristPose))
        {
            Debug.LogWarning("No wrist pose found! Skipping recognition.");
            return currentGesture;
        }

        foreach (var gesture in gestures)
        {
            if (gesture.fingerData.Count != 9)
            {
                Debug.LogWarning("Gesture data does not match expected joint count. Skipping gesture.");
                continue;
            }

            float sumDistance = 0;
            bool isDiscarded = false;

            for (int i = 0; i < gesture.fingerData.Count; i++)
            {
                XRHandJointID[] trackedJoints =
                {
                    XRHandJointID.ThumbTip, XRHandJointID.IndexTip, XRHandJointID.MiddleTip,
                    XRHandJointID.RingTip, XRHandJointID.LittleTip,
                    XRHandJointID.IndexIntermediate, XRHandJointID.MiddleIntermediate,
                    XRHandJointID.RingIntermediate, XRHandJointID.LittleIntermediate
                };

                XRHandJointID jointID = trackedJoints[i];
                XRHandJoint joint = hand.GetJoint(jointID);

                if (!joint.TryGetPose(out Pose fingerPose))
                {
                    Debug.LogWarning($"Failed to get pose for joint {jointID}. Skipping this joint.");
                    isDiscarded = true;
                    break;
                }
                //float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                if (joint.TryGetPose(out Pose pose))
                {
                    Vector3 relativePosition = pose.position - wristPose.position;
                    float distance = Vector3.Distance(relativePosition, gesture.fingerData[i]);

                    Debug.Log($"Comparing {jointID} - Distance: {distance}");  // Debug log for gesture comparison


                    if (distance > threshold)
                    {
                        isDiscarded = true;
                        break;
                    }

                    sumDistance += distance;
                }
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentGesture = gesture;
            }
        }
        return currentGesture;
    }
}