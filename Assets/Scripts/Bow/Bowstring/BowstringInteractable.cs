using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class BowstringInteractable : XRGrabInteractable {
    [SerializeField] float restDistanceTolerance;
    [SerializeField] float grabDistanceLimit;
    [SerializeField] float innerXDrawLimit;
    [SerializeField] float outerXDrawLimit;
    [SerializeField] XRSocketInteractor arrowSocket;

    Transform parent;
    Vector3 restingPosition;
    Rigidbody rb;
    IXRSelectInteractor interactor;
    GameObject socketedArrow;
    float updatedXPosition;
    float bowstringPullPercentage;
    string defaultLayer = "Default";
    string grabLayer = "Grab";
    bool isGrabbed = false;
    bool isReleased = false;

    void Start() {
        parent = transform.parent;
        restingPosition = transform.localPosition;
        rb = GetComponent<Rigidbody>();
        selectEntered.AddListener(MidpointGrabbed);
        selectExited.AddListener(MidpointReleased);
        if (arrowSocket != null) {
            arrowSocket.selectEntered.AddListener(ArrowSocketed);
            arrowSocket.selectExited.AddListener(ArrowUnsocketed);
        }
    }

    void Update() {
        // Need to ensure rotation is always the same
        transform.localRotation = Quaternion.identity;
        if (isGrabbed) {
            transform.parent = parent;
            updatedXPosition = transform.localPosition.x;
            if (updatedXPosition > innerXDrawLimit) {
                updatedXPosition = innerXDrawLimit;
            }
            else if (updatedXPosition < outerXDrawLimit) {
                updatedXPosition = outerXDrawLimit;
            }
            transform.localPosition = new Vector3(updatedXPosition, 0f, 0f);
            bowstringPullPercentage = updatedXPosition / outerXDrawLimit;

            float grabDistance = Vector3.Distance(transform.position, interactor.transform.position);
            if (grabDistance > grabDistanceLimit) {
                rb.isKinematic = false;
                SetLayerMask(defaultLayer);
            }
        }
        else if (isReleased) {
            if (socketedArrow != null) {
                socketedArrow.GetComponent<ArrowController>().FireArrow(bowstringPullPercentage);
                socketedArrow = null;
            }
            float distanceToRestingPosition = Vector3.Distance(transform.localPosition, restingPosition);
            if (distanceToRestingPosition <= restDistanceTolerance) {
                transform.localPosition = restingPosition;
                rb.isKinematic = true;
                SetLayerMask(grabLayer);
                isReleased = false;
            }
        }
    }

    void MidpointGrabbed(SelectEnterEventArgs arg) {
        interactor = arg.interactorObject;
        rb.isKinematic = true;
        isGrabbed = true;
    }

    void MidpointReleased(SelectExitEventArgs arg) {
        isGrabbed = false;
        rb.isKinematic = false;
        isReleased = true;
    }

    void ArrowSocketed(SelectEnterEventArgs arg) {
        socketedArrow = arg.interactableObject.transform.gameObject;
    }

    void ArrowUnsocketed(SelectExitEventArgs arg) {
        socketedArrow = null;
    }

    void SetLayerMask(string layerMask) {
        interactionLayers = InteractionLayerMask.GetMask(layerMask);
    }

    public float GetBowstringPullPercentage() {
        return bowstringPullPercentage;
    }
}
