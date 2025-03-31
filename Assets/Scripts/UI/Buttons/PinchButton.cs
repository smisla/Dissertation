using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PinchButton : MonoBehaviour
{
    public UnityEvent onPinchActivate;


    void Start()
    {
        XRGrabInteractable interactable = GetComponent<XRGrabInteractable>();

        if (interactable != null)
        {
            interactable.selectEntered.AddListener(interactor => StartCountdown());
        }
    }

    private void StartCountdown()
    {
        onPinchActivate.Invoke();
    }

}
