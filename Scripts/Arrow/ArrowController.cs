using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ArrowController : XRGrabInteractable {
    [Tooltip("The max speed the arrow can be fired at.")]
    [SerializeField] float maxArrowSpeed;
    [Tooltip("A small delay that stops the arrow's rotation from matching its velocity vector when initially fired.")]
    [SerializeField] float initialRotationDelay;
    [Tooltip("The distance at which the arrow is stopped in the air if player fires out of their turn.")]
    [SerializeField] float stoppingDistance;

    PlayerType arrowOwner;
    ArrowAudioController arrowAudioController;
    TurnManager turnManager;
    ArrowPool arrowPool;
    WindController windController;
    WarningDisplayController warningDisplayController;
    PointsManager pointsManager;
    Rigidbody rb;
    MeshCollider arrowCollider;
    bool isGrabbed;
    bool isMoving = false;
    bool canRotate = false;
    bool firedOutOfTurn;
    Vector3 initialFiringPosition;

    void Start() {
        arrowAudioController = FindAnyObjectByType<ArrowAudioController>();
        turnManager = FindAnyObjectByType<TurnManager>();
        arrowPool = FindAnyObjectByType<ArrowPool>();
        windController = FindAnyObjectByType<WindController>();
        pointsManager = FindAnyObjectByType<PointsManager>();

        warningDisplayController = FindAnyObjectByType<WarningDisplayController>();
        rb = GetComponent<Rigidbody>();
        arrowCollider = GetComponentInChildren<MeshCollider>();

        selectEntered.AddListener(ArrowGrabbed);
        selectExited.AddListener(ArrowReleased);
    }

    void Update() {
        if (isMoving && canRotate && rb.velocity != Vector3.zero) {
            // Sets the arrow's rotation to match its velocity vector while its moving
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }

        if (firedOutOfTurn) {
            StopAndDespawnArrow();
        }
    }

    void OnCollisionEnter(Collision other) {
        // Enable the despawn timer no matter what object an arrow hits, so it can return to the pool of inactive arrows
        // Need to check if it has the arrow tag, because simulation arrows are not part of the arrow pool
        if (transform.CompareTag("Arrow")) {
            arrowPool.InitiateDespawnTimer(gameObject);
            Debug.Log("Arrow hit a collider. Despawning");
        }

        if (isMoving && other != null) {
            // Allows the arrow to stick onto the first thing it collides with once it is in motion
            rb.isKinematic = true;
            interactionLayers = InteractionLayerMask.GetMask("Grab", "ArrowSocket");
            isMoving = false;
            canRotate = false;
            // Only update turn if its a live arrow (simulation arrows cant)
            if (transform.CompareTag("Arrow")) {
                turnManager.UpdateTurn();
                // Checking that the colliding object is the arrow tag, because simulation arrows will also collide with the target
                if (other.gameObject.CompareTag("PointsBoard")) {
                    arrowAudioController.PlayArrowHit();
                    // Collider disabled to avoid other arrows colliding with eachother on the target
                    arrowCollider.enabled = false;
                    windController.IncreaseArrowTargetHits();
                }
                else {
                    // Earned points are only shown when an arrow hits the target (so only when above 0)
                    // This allows earned points to be 0 if the arrow misses the target
                    pointsManager.IncreasePoints(0, arrowOwner);
                }
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
        // Check if any other arrows are currently in motion
        PlayerType arrowInMotionOwner = PlayerType.None;
        List<ArrowController> activeArrowControllers = arrowPool.GetActiveArrowControllers();
        foreach (ArrowController arrowController in activeArrowControllers) {
            if (arrowController.GetIsMoving()) {
                arrowInMotionOwner = arrowController.GetArrowOwner();
                break;
            }
        }

        // Fire the arrow
        isMoving = true;
        rb.isKinematic = false;
        interactionLayers = InteractionLayerMask.GetMask("Grab");
        float arrowSpeed = maxArrowSpeed * bowstringPullPercentage;
        rb.AddForce(transform.forward * arrowSpeed, ForceMode.Impulse);
        windController.InitializeWindEffect(gameObject);

        if (transform.CompareTag("Arrow")) {
            // Check if it is not the player's turn, and the current arrow being fired is owned by them
            // Check if the arrow in motion (if any is found) has the same owner as the current arrow being fired
            // Helps protect against arrow spam
            if (turnManager.CheckIfFiringOutOfTurn(arrowOwner) || (arrowInMotionOwner == arrowOwner)) {
                warningDisplayController.DisplayOrResetWarning();
                initialFiringPosition = transform.position;
                arrowCollider.enabled = false;
                firedOutOfTurn = true;
            }
            else {
                // Play arrow fire audio if it was a valid shot
                arrowAudioController.PlayArrowFire();
            }
        }
        StartCoroutine(ApplyInitialRotationDelay());
    }

    void StopAndDespawnArrow() {
        // If arrow was fired out of turn, we let it travel a little before stopping it mid-air and warning the player
        float distanceFromFiringPosition = Vector3.Distance(initialFiringPosition, transform.position);
        if (distanceFromFiringPosition >= stoppingDistance) {
            arrowPool.InitiateDespawnTimer(gameObject, true);
            isMoving = false;
            rb.isKinematic = true;
            interactionLayers = InteractionLayerMask.GetMask("Grab", "ArrowSocket");
            firedOutOfTurn = false;
        }
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

    public bool GetFiredOutOfTurn() {
        return firedOutOfTurn;
    }

    public void SetArrowOwner(PlayerType newArrowOwner) {
        arrowOwner = newArrowOwner;
    }

    public PlayerType GetArrowOwner() {
        return arrowOwner;
    }
}
