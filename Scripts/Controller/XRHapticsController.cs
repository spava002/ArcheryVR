using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHapticsController : MonoBehaviour {
    [Range(0f, 1f)] [SerializeField] float maxHapticsIntensity;
    [SerializeField] float hapticsDuration;

    XRBaseController controller;
    XRDirectInteractor controllerInteractor;
    BowstringInteractable bowstringInteractable;
    bool bowstringGrabbed;

    void Start() {
        controller = GetComponent<XRBaseController>();
        controllerInteractor = GetComponent<XRDirectInteractor>();
        controllerInteractor.selectEntered.AddListener(OnGrab);
        controllerInteractor.selectExited.AddListener(OnReleased);
    }

    void Update() {
        if (bowstringGrabbed && bowstringInteractable != null) {
            float scaledHapticsIntensity = maxHapticsIntensity * bowstringInteractable.GetBowstringPullPercentage();
            controller.SendHapticImpulse(scaledHapticsIntensity, hapticsDuration);
        }    
    }

    void OnGrab(SelectEnterEventArgs arg) {
        // Checks if the bowstring's midpoint (grab-able point) has been grabbed
        if (arg.interactableObject.transform.CompareTag("BowComponent")) {
            bowstringGrabbed = true;
            bowstringInteractable = arg.interactableObject.transform.GetComponent<BowstringInteractable>();
        }
    }

    void OnReleased(SelectExitEventArgs arg) {
        // Checks if the bowstring's midpoint (grab-able point) has been released
        if (arg.interactableObject.transform.CompareTag("BowComponent")) {
            bowstringGrabbed = false;
            bowstringInteractable = null;
        }
    }
}
