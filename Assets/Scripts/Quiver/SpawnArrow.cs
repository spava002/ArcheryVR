using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnArrow : MonoBehaviour {
    [SerializeField] Transform rightController;

    XRDirectInteractor controllerInteractor;
    float interactionRadius;
    ArrowPool arrowPool;
    ArrowController arrowController = null;
    bool controllerInRange = false;
    bool controllerIsGrabbing = false;
    GameObject arrow;

    void Start() {
        interactionRadius = GetComponent<SphereCollider>().radius;
        arrowPool = FindAnyObjectByType<ArrowPool>();

        if (rightController != null) {
            controllerInteractor = rightController.GetComponent<XRDirectInteractor>();
            controllerInteractor.selectEntered.AddListener(IsGrabbed);
            controllerInteractor.selectExited.AddListener(IsReleased);
        }
    }

    void Update() {
        CheckIfInRange();
        // If we are currently grabbing something, and in range, we cannot get another arrow reference
        if (controllerInRange && controllerIsGrabbing && arrow == null) {
            Debug.Log("Cannot display another arrow while already holding something.");
        }
        else {
            if (controllerInRange) {
                // Set a new arrow reference, if none is set already
                if (arrow == null) {
                    arrow = arrowPool.GetAvailableArrow();
                }

                // Check that an arrow was assigned, otherwise, no arrows are available from the pool
                if (arrow != null) {
                    arrow.SetActive(true);
                    arrow.GetComponent<Rigidbody>().isKinematic = true;
                    arrowController = arrow.GetComponent<ArrowController>();
                    // Mirror the controller's transform onto the arrow, so its always in a grab-able position
                    arrow.transform.position = rightController.position;
                    // To keep the arrow in a grab-able rotation, we match its rotation to the camera
                    float cameraYRotation = transform.parent.rotation.eulerAngles.y;
                    arrow.transform.rotation = Quaternion.Euler(new Vector3(135f, cameraYRotation, -90f));
                }
                else {
                    Debug.LogError("More arrows need to be instantiated.");
                }
            }
            else if (!controllerInRange && arrow != null){
                // Remove the arrow's reference from the quiver if it was grabbed and pulled outside the range
                if (arrowController.GetIsGrabbed()) {
                    arrowController.SetArrowOwner(PlayerType.Player1);
                    arrow = null;
                    arrowController = null;
                }
                // De-activate the arrow to make it hidden, but maintain its reference for next time
                else {
                    arrow.SetActive(false);
                }
            }
        }
    }

    void CheckIfInRange() {
        // Only allowing right controller to grab arrows for now, since it only makes sense to be grabbing arrow with right hand
        float rightControllerDistance = Vector3.Distance(rightController.position, transform.position);
        if (rightControllerDistance <= interactionRadius) {
            controllerInRange = true;
        }
        else {
            controllerInRange = false;
        }
    }

    void IsGrabbed(SelectEnterEventArgs arg) {
        controllerIsGrabbing = true;
    }

    void IsReleased(SelectExitEventArgs arg) {
        controllerIsGrabbing = false;
    }
}
