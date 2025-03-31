//using UnityEngine;
//using System.Collections;
//using TMPro;
//using UnityEngine.UI;
//using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.XR.Interaction.Toolkit.Interactables;

//public class PlayButton :  MonoBehaviour
//{
//    public Button playButton;
//    public CountdownTimer countdownTimer;
//    public XRGrabInteractable interactable;

//    private void Start()
//    {
//        interactable = playButton.GetComponent<XRGrabInteractable>();

//        if (interactable != null )
//        {
//            interactable.selectEntered.AddListener(OnPinchPlay);
//        }

//        playButton.onClick.AddListener(OnPlayButtonClicked);
//    }

//    private void OnPlayButtonClicked()
//    {
//        StartCountdown();
//    }

//    private void OnPinchPlay(SelectEnterEventArgs args)
//    {
//        StartCountdown();
//    }

//    private void StartCountdown()
//    {
//        playButton.gameObject.SetActive(false);
//        countdownTimer.gameObject.SetActive(true);
//        countdownTimer.OnEnable();
//    }

//}
