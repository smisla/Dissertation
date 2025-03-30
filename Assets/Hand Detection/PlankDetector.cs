//using UnityEngine;
//using UnityEngine.XR.Hands;
//using UnityEngine.XR.Management;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Events;
//using TMPro;
//using Unity.XR.CoreUtils;

////[System.Serializable]
//public struct Gesture
//{
//    public string name;
//    public List<Vector3> fingerData;
//    public UnityEvent onRecognised;

//}

//public class PlankDetector : MonoBehaviour
//{
//    // Provides access to the hand tracking system
//    public TextMeshProUGUI gestureText;
//    public TextMeshProUGUI instructionText;
//    public Transform headset;

//    public float threshold = 0.1f;
//    public float minHeight, maxHeight;
//    public bool isPlanking = false;
//    private bool calibrationComplete = false;

//    public XRHandSubsystem handSubsystem;
//    public List<Gesture> gestures;
//    private Gesture previousLeftGesture, previousRightGesture;
//    private string leftHandGesture = "None", rightHandGesture = "None";



//    private void Start()
//    {
//        //Get the active XR hand tracking subsystem
//        handSubsystem = XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRHandSubsystem>();
//        previousLeftGesture = new Gesture();
//        previousRightGesture = new Gesture();  
//        // previousGesture = new Gesture();
//        if (handSubsystem == null)
//        {
//            Debug.LogError("XRHandSubsystem not found! Ensure XR Hands package is installed and enabled.");
//        }
//        else
//        {
//            Debug.Log("XRHandSubsystem successfully loaded.");
//        }
//    }

//    private void Update()
//    {

//        if (Input.GetKeyUp(KeyCode.L))
//        {
//            SaveGesture(handSubsystem.leftHand, leftHandGesture, "Left Hand");
//            Debug.Log("Saving LEFT HAND Gesture...");
//        }

//        if (Input.GetKeyUp(KeyCode.R))
//        {
//            SaveGesture(handSubsystem.rightHand, rightHandGesture, "Right Hand");
//            Debug.Log("Saving RIGHT HAND Gesture...");
//        }

//        Gesture currentLeftGesture = Recognise(handSubsystem.leftHand, leftHandGesture);
//        Gesture currentRightGesture = Recognise(rightHandGesture, rightHandGesture);

//        leftHandGesture = currentLeftGesture.name;
//        rightHandGesture = currentRightGesture.name;

//        //Gesture currentGesture = Recognise();

//        bool leftRecognised = !currentLeftGesture.Equals(new Gesture());
//        bool rightRecognised = !currentRightGesture.Equals(new Gesture());


//        //bool hasRecognised = !currentGesture.Equals(new Gesture());

//        if (leftRecognised && currentLeftGesture.Equals(previousLeftGesture))
//        {
//            previousLeftGesture = currentLeftGesture;
//            currentLeftGesture.onRecognised.Invoke();
//        }

//        if (rightRecognised && currentRightGesture.Equals(previousRightGesture))
//        {
//            previousRightGesture = currentRightGesture;
//            currentRightGesture.onRecognised.Invoke();
//        }

//        if (!isCalibrating && !calibrationComplete && leftHandGesture == "Thumbs Up" && rightHandGesture == "Thumbs Up")
//        {
//            StartCoroutine(StartCalibration());
//        }

//        if (calibrationComplete)
//        {
//            CheckPlankBreak();
//        }

//        //if (hasRecognised && currentGesture.Equals(previousGesture))
//        //{
//        //    Debug.Log("New Gesture Found: " + currentGesture.name);
//        //    previousGesture = currentGesture;
//        //    currentGesture.onRecognised.Invoke();
//        //}

//        if (gestureText != null)
//        {
//            gestureText.text = "Left Gesture: " + currentLeftGesture.name + " | Right Gesture " + currentRightGesture.name;
//        }

//        else
//        {
//            if (gestureText != null)
//            {
//                gestureText.text = "Gesture: None";
//            }
//        }
//    }

//    private IEnumerator StartCalibration()
//    {
        
//        isCalibrating = true;
//        instructionText.text = "Get ready for calibration... put your knees down if needed.";
//        yield return new WaitForSeconds(3);

//        instructionText.text = "Look down for 5 seconds...";
//        yield return new WaitForSeconds(5);
//        minHeight = Camera.main.transform.position.y;

//        instructionText.text = "Look up for 5 seconds...";
//        yield return new WaitForSeconds(5);
//        maxHeight = Camera.main.transform.position.y;

//        calibrationComplete = true;
//        instructionText.text = "Calibration complete! Get into plank position.";
        
//    }

//    public void SaveGesture(XRHand hand, string gesture, string name)
//    {

//        if (hand == null || !hand.isTracked)
//        {
//            Debug.LogError($"{handName} not detected! Cannot save gesture.");
//            return;
//        }

//        Gesture g = new Gesture();
//        g.name = handName + "Gesture" + (gestureList.Count + 1);
//        List<Vector3> data = new List<Vector3>();

//        SaveHandData(hand, data);

//        //XRHand leftHand = handSubsystem.leftHand;
//        //XRHand rightHand = handSubsystem.rightHand;

//        g.fingerData = data;
//        gestures.Add(g);
//        Debug.Log($"{handName} saved successfully! Recorded {g.fingerData.Count} joint positions.");
//    }

//    private void SaveHandData(XRHand hand, List<Vector3> data)
//    {
//        XRHandJoint wristJoint = hand.GetJoint(XRHandJointID.Wrist);

//        if (!wristJoint.TryGetPose(out Pose wristPose))
//        {
//            Debug.LogError("Failed to get wrist pose.");
//            return;
//        }

//        foreach (XRHandJointID jointID in new XRHandJointID[]
//        {
//            XRHandJointID.ThumbTip, XRHandJointID.IndexTip, XRHandJointID.MiddleTip,
//            XRHandJointID.RingTip, XRHandJointID.LittleTip,
//            XRHandJointID.IndexIntermediate, XRHandJointID.MiddleIntermediate,
//            XRHandJointID.RingIntermediate, XRHandJointID.LittleIntermediate
//        })
//        {
//            XRHandJoint joint = hand.GetJoint(jointID);
//            if (joint.TryGetPose(out Pose pose))
//            {
//                Vector3 relativePosition = pose.position - wristPose.position;
//                data.Add(relativePosition);
//            }
//        }
//    }

//    Gesture Recognise()
//    {
//        Gesture currentGesture = new Gesture();
//        float currentMin = Mathf.Infinity;


//        //XRHand leftHand = handSubsystem.leftHand;
//        //XRHand rightHand = handSubsystem.rightHand;

//        //XRHand hand = leftHand.isTracked ? leftHand : rightHand;

//        if (hand == null || !hand.isTracked)
//        {
//            Debug.LogWarning("No valid hand detected");
//            return currentGesture;
//        }

//        XRHandJoint wristJoint = hand.GetJoint(XRHandJointID.Wrist);
//        Pose wristPose = new Pose();

//        if (!wristJoint.TryGetPose(out wristPose))
//        {
//            Debug.LogWarning("No wrist pose found! Skipping recognition.");
//            return currentGesture;
//        }

//        foreach (var gesture in gestureList)
//        {
//            if (gesture.fingerData.Count != 9)
//            {
//                Debug.LogWarning("Gesture data does not match expected joint count. Skipping gesture.");
//                continue;
//            }

//            float sumDistance = 0;
//            bool isDiscarded = false;

//            for (int i = 0; i < gesture.fingerData.Count; i++)
//            {
//                XRHandJointID[] trackedJoints =
//                {
//                    XRHandJointID.ThumbTip, XRHandJointID.IndexTip, XRHandJointID.MiddleTip,
//                    XRHandJointID.RingTip, XRHandJointID.LittleTip,
//                    XRHandJointID.IndexIntermediate, XRHandJointID.MiddleIntermediate,
//                    XRHandJointID.RingIntermediate, XRHandJointID.LittleIntermediate
//                };

//                //XRHandJointID jointID = trackedJoints[i];
//                XRHandJoint joint = hand.GetJoint(trackedJoints[i]);

//                if (!joint.TryGetPose(out Pose fingerPose))
//                {
//                    Debug.LogWarning($"Failed to get pose for joint {jointID}. Skipping this joint.");
//                    isDiscarded = true;
//                    break;
//                }

//                //float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);

//                Vector3 relativePosition = pose.position - wristPose.position;
//                float distance = Vector3.Distance(relativePosition, gesture.fingerData[i]);

//                //Debug.Log($"Comparing {jointID} - Distance: {distance}");  // Debug log for gesture comparison


//                if (distance > threshold)
//                {
//                    isDiscarded = true;
//                    break;
//                }

//                sumDistance += distance;
                
//            }

//            if (!isDiscarded && sumDistance < currentMin)
//            {
//                currentMin = sumDistance;
//                currentGesture = gesture;
//            }
//        }
//        return currentGesture;
//    }


//    public void StartPlank()
//    {
//        minHeight = Camera.main.transform.position.y;
//        maxHeight = minHeight;
//        isPlanking = true;
//        Debug.Log("Plank started! Calibrating height range...");
//    }

//    public void SetMinHeight()
//    {
//        minHeight = Camera.main.transform.position.y;
//        Debug.Log($"Min Height Set: {minHeight}"); 
//    }

//    public void SetMaxHeight()
//    {
//        maxHeight = Camera.main.transform.position.y;
//        Debug.Log($"Max Height Set : {maxHeight}");
//    }

//    private void CheckPlankBreak()
//    {
//        if (!isPlanking)
//        {
//            Debug.LogError("No Plank Detected");
//            return;
//        }

//        float headsetHeight = Camera.main.transform.position.y;
//        bool heightBroken = headsetHeight > maxHeight + heightTolerance || headsetHeight < minHeight - heightTolerance;
//        bool handsOpen = leftHandGesture == "Open" && rightHandGesture == "Open";

//        if (heightBroken && handsOpen)
//        {
//            Debug.Log("Plank broken! Height and hand gestures detected a break.");
//            isPlanking = false;
//        }
//    }
//}