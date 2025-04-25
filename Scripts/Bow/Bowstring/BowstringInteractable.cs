using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowstringInteractable : XRGrabInteractable {
    [SerializeField] float restDistanceTolerance;
    [SerializeField] float grabDistanceLimit;
    [SerializeField] float innerXDrawLimit;
    [SerializeField] float outerXDrawLimit;
    [SerializeField] XRSocketInteractor arrowSocket;

    Rigidbody rb;
    BowstringAudioController bowstringAudioController;
    Transform parent;
    Vector3 restingPosition;
    IXRSelectInteractor interactor;
    GameObject socketedArrow;
    float updatedXPosition;
    float bowstringPullPercentage;
    string defaultLayer = "Default";
    string grabLayer = "Grab";
    bool isGrabbed = false;
    bool isReleased = false;

    void Start() {
        rb = GetComponent<Rigidbody>();
        bowstringAudioController = GetComponent<BowstringAudioController>();

        parent = transform.parent;
        restingPosition = transform.localPosition;
        
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
            // Start playing bowstring pull audio
            bowstringAudioController.PlayBowstringPull();
            
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

            // If the player pulls their hand outside of the allowed grab distance, then we automatically release the bowstring
            // Checking if we have an interactor, because this does not apply to the AI Opponent, which handles it differently.
            if (interactor != null) {
                float grabDistance = Vector3.Distance(transform.position, interactor.transform.position);
                if (grabDistance > grabDistanceLimit) {
                    rb.isKinematic = false;
                    SetLayerMask(defaultLayer);
                }
            }
        }
        else if (isReleased) {
            // Stop bowstring pull audio
            bowstringAudioController.StopBowstringPull();

            // Ensures if the player brings the bowstring back to resting position with an array socketed, it doesnt fire the arrow
            if (bowstringPullPercentage > Mathf.Epsilon && socketedArrow != null) {
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

    // Allows the AI Opponent to call this method, since it isn't able to interact with the bowstring using SelectExitEventArgs
    public void MidpointGrabbed() {
        isGrabbed = true;
    }

    void MidpointGrabbed(SelectEnterEventArgs arg) {
        interactor = arg.interactorObject;
        rb.isKinematic = true;
        isGrabbed = true;
    }

    // Allows the AI Opponent to call this method, since it isn't able to interact with the bowstring using SelectExitEventArgs
    public void MidpointReleased() {
        isGrabbed = false;
        rb.isKinematic = false;
        isReleased = true;
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

    public float GetOuterXDrawLimit() {
        return outerXDrawLimit;
    }
}
