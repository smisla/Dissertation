//using UnityEngine;
//using TMPro;

//public class TextFollow : MonoBehaviour
//{
//    [UnityEngine.SerializeField]
//    public Transform headset;

//    public Vector3 offset = new Vector3(0, -0.2f, 0.5f);
    
//    void Update()
//    {
//        if (headset != null) return;

//        transform.position = headset.position + headset.TransformDirection(offset);

//        transform.rotation = Quaternion.LookRotation(transform.position - headset.position);
//    }
//}
