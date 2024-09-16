using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ArrowController : XRGrabInteractable {
    [SerializeField] float maxArrowSpeed;
    [SerializeField] float initialRotationDelay;
    [SerializeField] float stoppingDistance;


    PlayerType arrowOwner;
    TurnManager turnManager;
    ArrowPool arrowPool;
    WindController windController;
    Rigidbody rb;
    Collider arrowCollider;
    bool isGrabbed;
    bool isMoving = false;
    bool canRotate = false;
    bool firedOutOfTurn;
    Vector3 initialFiringPosition;

    void Start() {
        turnManager = FindAnyObjectByType<TurnManager>();
        arrowPool = FindAnyObjectByType<ArrowPool>();
        windController = FindAnyObjectByType<WindController>();
        rb = GetComponent<Rigidbody>();
        arrowCollider = GetComponentInChildren<Collider>();
        selectEntered.AddListener(ArrowGrabbed);
        selectExited.AddListener(ArrowReleased);
    }

    void Update() {
        if (isMoving && canRotate && rb.velocity != Vector3.zero) {
            // Sets the arrow's rotation to match its velocity vector
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }

        // If arrow was fired out of turn, we let it travel a little before stopping it mid-air and warning the player
        if (firedOutOfTurn) {
            float distanceFromFiringPosition = Vector3.Distance(initialFiringPosition, transform.position);
            if (distanceFromFiringPosition >= stoppingDistance) {
                // Need AlertPlayer() or something here, that displays a warning/visual indicator on a screen
                arrowPool.InitiateDespawnTimer(gameObject);
                rb.isKinematic = true;
                interactionLayers = InteractionLayerMask.GetMask("Grab", "ArrowSocket");
                firedOutOfTurn = false;
            }
        }
    }

    void OnCollisionEnter(Collision other) {
        // Enable the despawn timer no matter what object an arrow hits, so it can return to the pool of inactive arrows
        // Need to check if it has the arrow tag, because simulation arrows are not part of the arrow pool
        if (transform.CompareTag("Arrow")) {
            arrowPool.InitiateDespawnTimer(gameObject);
        }

        if (isMoving && other != null) {
            // Allows the arrow to stick onto the first thing it collides with once it is in motion
            rb.isKinematic = true;
            interactionLayers = InteractionLayerMask.GetMask("Grab", "ArrowSocket");
            isMoving = false;
            canRotate = false;
            // Stops simulation arrows from making updates in the turn manager
            if (transform.CompareTag("Arrow")) {
                turnManager.UpdateTurn();
            }
            // Checking that the colliding object is the arrow tag, because simulation arrows will also collide with the target
            if (other.gameObject.CompareTag("PointsBoard") && transform.CompareTag("Arrow")) {
                // Collider disabled to avoid other arrows colliding with eachother on the target
                arrowCollider.enabled = false;
                windController.IncreaseArrowTargetHits();
            }
        }
    }

    void ArrowGrabbed(SelectEnterEventArgs arg) {
        isGrabbed = true;
    }

    void ArrowReleased(SelectExitEventArgs arg) {
        isGrabbed = false;
        rb.isKinematic = false;
    }

    public void FireArrow(float bowstringPullPercentage) {
        isMoving = true;
        rb.isKinematic = false;
        // Might want to modify interaction layers within a seperate method
        // Also one on line 43
        interactionLayers = InteractionLayerMask.GetMask("Grab");
        float arrowSpeed = maxArrowSpeed * bowstringPullPercentage;
        rb.AddForce(transform.forward * arrowSpeed, ForceMode.Impulse);
        windController.InitializeWindEffect(gameObject);

        // Stops the arrow early (if its not a simulation arrow) within Update loop if the player has fired an arrow while it isnt their turn
        if (transform.CompareTag("Arrow") && turnManager.CheckIfFiringOutOfTurn(arrowOwner)) {
            initialFiringPosition = transform.position;
            firedOutOfTurn = true;
        }
        StartCoroutine(ApplyInitialRotationDelay());
    }

    // Small time delay applied before we reflect the velocity's direction onto the arrow's rotation
    // This is done because of high fluctuation in the rotation within the first frames of firing the arrow
    // This issue can disrupt the rotation, which in turn affects the direction the force is applied
    IEnumerator ApplyInitialRotationDelay() {
        yield return new WaitForSeconds(initialRotationDelay);
        canRotate = true;
    }

    public bool GetIsGrabbed() {
        return isGrabbed;
    }

    public bool GetIsMoving() {
        return isMoving;
    }

    public void SetArrowOwner(PlayerType newArrowOwner) {
        arrowOwner = newArrowOwner;
    }

    public PlayerType GetArrowOwner() {
        return arrowOwner;
    }
}
