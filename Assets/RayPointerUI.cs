//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.XR.Interaction.Toolkit.Interactors;

//public class RayPointerUI : MonoBehaviour
//{
//    public XRRayInteractor leftRayInteractor;
//    public XRRayInteractor rightRayInteractor;

//    public RectTransform LpointerUI;
//    public RectTransform RpointerUI;

//    void Update()
//    {
//        if (leftRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit Lhit))
//        {
//            LpointerUI.position = Lhit.point;
//            LpointerUI.gameObject.SetActive(true);
//        }
//        else
//        {
//            LpointerUI.gameObject.SetActive(false);
//        }

//        if (rightRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit Rhit))
//        {
//            RpointerUI.position = Rhit.point;
//            RpointerUI.gameObject.SetActive(true);
//        }
//        else
//        {
//            RpointerUI.gameObject.SetActive(false);
//        }

//    }
//}
