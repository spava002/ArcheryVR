using UnityEngine;

public class Button : MonoBehaviour {
    public enum ButtonType {
        ExitMatch,
        RecoverBow
    }

    [Header("Button Properties")]
    [Tooltip("Defines the function of the button when clicked.")]
    [SerializeField] ButtonType buttonType;

    [Header("Button Distance Limits")]
    [Tooltip("Highest distance the button can go from the resting position.")]
    [SerializeField] float maxY;
    [Tooltip("Lowest distance the button can go from the resting position.")]
    [SerializeField] float minY;

    [Tooltip("How much percent of the minY distance the button has to cover for the button's action to be applied.")]
    [SerializeField] [Range(0f, 1f)] float actionThresholdMultiplier;

    [Header("Audio")]
    [SerializeField] AudioClip buttonClick;
    [SerializeField] [Range(0f, 1f)] float audioVolume;

    Vector3 interactorEnterPosition;
    Vector3 interactorCurrentPosition;

    Rigidbody rb;
    BoxCollider buttonCollider;
    AudioSource audioSource;
    TurnManager turnManager;
    GameObject playerBow;
    Vector3 playerBowInitialPosition;
    Quaternion playerBowInitialRotation;
    Vector3 restingPosition;
    float drawbackActivationThreshold;
    // Provides a small enough value that when the button's distance is less than, then we automatically snap to the resting position to prevent further oscillations
    const float restingDistanceThreshold = 0.01f;
    bool inMotion;
    bool validButtonPress;
    bool buttonClickPlayed;

    void Start() {
        rb = GetComponent<Rigidbody>();
        buttonCollider = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();

        turnManager = FindAnyObjectByType<TurnManager>();
        playerBow = FindAnyObjectByType<BowInteractable>().gameObject;

        playerBowInitialPosition = playerBow.transform.position;
        playerBowInitialRotation = playerBow.transform.rotation;

        // Disable physics so the spring joint doesn't have any influence on the button
        // Physics only enabled when butotn is released
        rb.isKinematic = true;
        restingPosition = transform.position;
        // Defines how much the interactor's trigger collider needs to go inside of the button's collider before we starting pushing the button down
        // In this case its when the interactor is 1/4 into the button's collider
        drawbackActivationThreshold = buttonCollider.size.y * 0.75f;
    }

    void Update() {
        // Applies the button action if the activation threshold has been met
        // Also gives an audio cue to signify the button has registered
        if (validButtonPress && !buttonClickPlayed) {
            audioSource.PlayOneShot(buttonClick, audioVolume);
            buttonClickPlayed = true;
        }

        // Resets the button back to resting position once it has sprung back close enough
        if (inMotion) {
            ResetToRestingPosition();
        }
    }

    void OnTriggerStay(Collider other) {
        float interactorToButtonDistance = Vector3.Distance(other.transform.position, transform.position);
        if (interactorToButtonDistance <= drawbackActivationThreshold) {
            if (interactorEnterPosition == Vector3.zero) {
                interactorEnterPosition = other.transform.position;
                // Debug.Log("Interactor Entered at: " + interactorEnterPosition);
            }

            interactorCurrentPosition = other.transform.position;
            // Debug.Log("Interactor touching button at: " + interactorCurrentPosition);

            float yOffset = interactorEnterPosition.y - interactorCurrentPosition.y;
            float updatedY = Mathf.Clamp(restingPosition.y - yOffset, restingPosition.y - minY, restingPosition.y + maxY);

            // Update the button's position
            transform.position = new Vector3(restingPosition.x, updatedY, restingPosition.z);
        }
    }

    void OnTriggerExit(Collider other) {
        // Debug.Log("Interactor Exited");

        CheckIfValidButtonPress();

        interactorEnterPosition = Vector3.zero;
        // Enable physics, so the spring joint bounces the button back up to resting position
        rb.isKinematic = false;
        // Disable collider, so that the interactor can't touch the button while its moving back to rest
        buttonCollider.enabled = false;
        inMotion = true;
    }

    void CheckIfValidButtonPress() {
        float buttonDrawbackDistance = Vector3.Distance(transform.position, restingPosition);

        // If the button was released at a high enough percentage of its minY, then we can apply the button's action
        // This serves as a protection against accidentally touching the button and having its action applied
        validButtonPress = buttonDrawbackDistance >= minY * actionThresholdMultiplier;
    }

    void ApplyButtonAction() {
        switch (buttonType) {
            case ButtonType.RecoverBow:
                // Reset the bow's position to its original position and rotation
                playerBow.transform.position = playerBowInitialPosition;
                playerBow.transform.rotation = playerBowInitialRotation;
                break;
            case ButtonType.ExitMatch:
                // Exit the match early
                turnManager.EndGame();
                break;
        }
        validButtonPress = false;
        buttonClickPlayed = false;
    }

    void ResetToRestingPosition() {
        // Using distance instead of modifying Damper in spring joint component, creates a more realistic button spring effect
        float distanceToRestingPosition = Vector3.Distance(transform.position, restingPosition);
        if (distanceToRestingPosition <= restingDistanceThreshold) {
            rb.isKinematic = true;
            transform.position = restingPosition;
            buttonCollider.enabled = true;
            inMotion = false;
            // Applying button action only when its back at rest because the ExitMatch button becomes deactivated, so we need it to return to rest before that occurs
            if (validButtonPress) {
                ApplyButtonAction();
            }
        }
    }
}
