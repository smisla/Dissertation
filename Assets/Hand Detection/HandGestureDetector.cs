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

public class HandGestureDetector : MonoBehaviour
{
    // Provides access to the hand tracking system
    public TextMeshProUGUI gestureText;

    public float threshold = 0.1f;
    public XRHandSubsystem handSubsystem;
    public List<Gesture> leftHandGesture;
    public List<Gesture> rightHandGesture;

    public string currentLeftGestureName {  get; private set; }
    public string currentRightGestureName {  get; private set; }

    private bool gestureSystemInitialised = false;

    public bool IsReady()
    {
        return gestureSystemInitialised;
    }
    public bool debugMode = true;
    private Gesture previousGesture;

    private void Start()
    {
        //Get the active XR hand tracking subsystem
        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
        previousGesture = new Gesture();
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

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Save(isLeftHand: true);
            Debug.Log("Saving Left Gesture...");
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            Save(isLeftHand: false);
            Debug.Log("Saving Right Gesture...");
        }

        Gesture leftGesture = Recognise(true);
        Gesture rightGesture = Recognise(false);

        currentLeftGestureName = leftGesture.name;
        currentRightGestureName = rightGesture.name;

        if (gestureText != null)
        {
            gestureText.text = $"Left: {leftGesture.name} | Right: {rightGesture.name}";
        }
    }

    void Save(bool isLeftHand)
    {
        Gesture g = new Gesture();
        g.name = isLeftHand ? "Left Gesture" : "Right Gesture";
        List<Vector3> data = new List<Vector3>();

        XRHand hand = isLeftHand ? handSubsystem.leftHand : handSubsystem.rightHand;

        //XRHand leftHand = handSubsystem.leftHand;
        //XRHand rightHand = handSubsystem.rightHand;

        if (!hand.isTracked)
        {
            Debug.LogError($"{(isLeftHand ? "Left" : "Right")} hand not tracked!");
            return;
        }

        SaveHandData(hand, data);
        g.fingerData = data;

        if (isLeftHand)
        {
            leftHandGesture.Add(g);
        }
        else
        {
            rightHandGesture.Add(g);
        }

        Debug.Log($"{(isLeftHand ? "Left" : "Right")} hand gesture saved with {data.Count} joint positions.");
        //if (leftHand.isTracked)
        //{
        //    Debug.Log("Left hand detected. Saving...");
        //    SaveHandData(leftHand, data);
        //}
        //else if (rightHand.isTracked)
        //{
        //    Debug.Log("Right hand detected. Saving...");
        //    SaveHandData(rightHand, data);
        //}

        //if (!leftHand.isTracked && !rightHand.isTracked)
        //{
        //    Debug.LogError("No hand detected when trying to save!");
        //    return;
        //}

        //g.fingerData = data;
        //leftHandGesture.Add(g);
        //Debug.Log("Gesture saved successfully! Recorded " + g.fingerData.Count + " joint positions.");
    }

    private void SaveHandData(XRHand hand, List<Vector3> data)
    {
        XRHandJoint wristJoint = hand.GetJoint(XRHandJointID.Wrist);

        if (!wristJoint.TryGetPose(out Pose wristPose))
        {
            Debug.LogError("Failed to get wrist pose.");
            return;
        }

        Quaternion wristRotation = wristPose.rotation;

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
                Vector3 worldRelative = pose.position - wristPose.position;
                Vector3 localRelative = Quaternion.Inverse(wristRotation) * worldRelative;
                data.Add(localRelative);
                Debug.Log($"Saved {jointID} local to wrist: {localRelative}");
            }
        }
    }

    Gesture Recognise(bool isLeftHand)
    {
        Gesture currentGesture = new Gesture();
        float currentMin = Mathf.Infinity;

        XRHand hand = isLeftHand ? handSubsystem.leftHand : handSubsystem.rightHand;

        if (!hand.isTracked) return currentGesture;

        if (!hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out Pose wristPose)) return currentGesture;

        List<Gesture> gestureList = isLeftHand ? leftHandGesture : rightHandGesture;

        foreach (var gesture in gestureList)
        {
            if (gesture.fingerData.Count != 9) continue;

            float sumDistance = 0f;
            bool isDiscarded = false;

            XRHandJointID[] trackedJoints =
                {
                    XRHandJointID.ThumbTip, XRHandJointID.IndexTip, XRHandJointID.MiddleTip,
                    XRHandJointID.RingTip, XRHandJointID.LittleTip,
                    XRHandJointID.IndexIntermediate, XRHandJointID.MiddleIntermediate,
                    XRHandJointID.RingIntermediate, XRHandJointID.LittleIntermediate
                };

            for (int i = 0; i < trackedJoints.Length; i++)
            {
                if (!hand.GetJoint(trackedJoints[i]).TryGetPose(out Pose pose)) { isDiscarded = true; break; }

                Vector3 worldRelative = pose.position - wristPose.position;
                Vector3 localRelative = Quaternion.Inverse(wristPose.rotation) * worldRelative;
                float distance = Vector3.Distance(localRelative, gesture.fingerData[i]);

                if (distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
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