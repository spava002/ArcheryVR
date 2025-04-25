using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabLimiter : XRGrabInteractable {
    [SerializeField] Transform doorHandle;
    [SerializeField] float distanceLimit;
    IXRSelectInteractor interactor;
    string defaultLayer = "Default";
    string grabLayer = "Grab";
    bool isGrabbed = false;

    void Start() {
        selectEntered.AddListener(OnGrabbed);
        selectExited.AddListener(OnReleased);
    }

    void Update() {
        if (isGrabbed && interactor != null) {
            float grabDistance = Vector3.Distance(doorHandle.position, interactor.transform.position);
            if (grabDistance > distanceLimit) {
                SetLayerMask(defaultLayer);
            }
        }
        else {
            SetLayerMask(grabLayer);
        }
    }

    void OnGrabbed(SelectEnterEventArgs arg) {
        interactor = arg.interactorObject;
        isGrabbed = true;
    }

    void OnReleased(SelectExitEventArgs arg) {
        interactor = null;
        isGrabbed = false;
    }

    void SetLayerMask(string layerMask) {
        interactionLayers = InteractionLayerMask.GetMask(layerMask);
    }
}
