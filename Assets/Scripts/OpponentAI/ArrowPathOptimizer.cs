using System;
using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Animations;

// This will be on an arrow (simulation arrow) that is seperate from the arrow prefab
public class ArrowPathOptimizer : MonoBehaviour {
    [SerializeField] int iterations;
    [SerializeField] float acceptableRange;

    GameObject target;
    Vector3 targetRelativePosition;
    ArrowController arrowController;
    Vector3 initialPosition;
    Quaternion initialRotation;
    float optimalYRotation;
    float optimalXRotation;
    Quaternion optimalRotation;
    bool yRotationConverged;
    bool xRotationConverged;
    int iterationCount;
    bool testFire;
    bool simulationRunning;
    float elapsedTime = 0f;

    void Start() {
        target = GameObject.FindGameObjectWithTag("PointsBoard");
        targetRelativePosition = new Vector3(0f, 1f, 25f);
        arrowController = GetComponent<ArrowController>();
    }

    void Update() {
        Debug.DrawRay(transform.position, transform.forward * 25f, Color.blue);
        if (simulationRunning) {
            elapsedTime += Time.deltaTime;
            FireTest();
        }
    }

    void OnCollisionEnter(Collision other) {
        // World coordinates of the location where the arrow hit the detector
        Vector3 hitPoint = other.contacts[0].point;
        // Horizontal/vertical arrow hit points relative to the target position
        Vector3 relativeHorizontalHitPoint = new Vector3(hitPoint.x - target.transform.position.x, targetRelativePosition.y, targetRelativePosition.z);
        Vector3 relativeVerticalHitPoint = new Vector3(targetRelativePosition.x, hitPoint.y - (target.transform.position.y - targetRelativePosition.y), targetRelativePosition.z);
        // Generate the angle between the relative hit directions and the target's relative position
        float newHorizontalFiringAngle = Vector3.Angle(targetRelativePosition, relativeHorizontalHitPoint);
        float newVerticalFiringAngle = Vector3.Angle(targetRelativePosition, relativeVerticalHitPoint);

        // Debug.Log("Horizontal Angle calculated with: " + targetRelativePosition + " and " + relativeHorizontalHitPoint);
        // Debug.Log("Vertical Angle calculated with: " + targetRelativePosition + " and " + relativeVerticalHitPoint);

        // Horizontal/vertical world positions on the detector of where the arrow hit 
        Vector3 worldHorizontalHitPoint = new Vector3(hitPoint.x, target.transform.position.y, target.transform.position.z);
        Vector3 worldVerticalHitPoint = new Vector3(target.transform.position.x, hitPoint.y, target.transform.position.z);
        // Local horizontal/vertical positions on the detector of where the arrow hit
        Vector3 localHorizontalHitPoint = other.transform.InverseTransformPoint(worldHorizontalHitPoint);
        Vector3 localVerticalHitPoint = other.transform.InverseTransformPoint(worldVerticalHitPoint);


        if (iterationCount < iterations && !(yRotationConverged && xRotationConverged)) {
            // Landed to the left of the target
            if (localHorizontalHitPoint.x > 0) {
                // Debug.Log(iterationCount + " New Horizontal Angle: " + newHorizontalFiringAngle);
                optimalYRotation += newHorizontalFiringAngle;
            }
            // Landed to the right of the target
            else if (localHorizontalHitPoint.x < 0) {
                // Debug.Log(iterationCount + " New Horizontal Angle: " + -newHorizontalFiringAngle);
                optimalYRotation -= newHorizontalFiringAngle;
            }

            // Landed above the target
            if (localVerticalHitPoint.y > 0) {
                // Debug.Log(iterationCount + " New Vertical Angle: " + newVerticalFiringAngle);
                optimalXRotation += newVerticalFiringAngle;
            }
            // Landed below the target
            else if (localVerticalHitPoint.y < 0) {
                // Debug.Log(iterationCount + " New Vertical Angle: " + -newVerticalFiringAngle);
                optimalXRotation -= newVerticalFiringAngle;
            }

            yRotationConverged = Mathf.Abs(newHorizontalFiringAngle) <= acceptableRange;
            // Debug.Log("Y Converged: " + yRotationConverged);
            xRotationConverged = Mathf.Abs(newVerticalFiringAngle) <= acceptableRange;
            // Debug.Log("X Converged: " + xRotationConverged);

            iterationCount++;
            testFire = true;
        }
        else {
            if (Mathf.Abs(newHorizontalFiringAngle) <= acceptableRange && Mathf.Abs(newVerticalFiringAngle) <= acceptableRange) {
                // Debug.Log("Bullseye!");
                // Debug.Log("Took " + iterationCount + " iterations.");
            }
            else if (iterationCount == iterations) {
                // Debug.Log("Maybe try adding more iterations.");
            }
            simulationRunning = false;
            // Debug.Log(elapsedTime);
        }
    }

    void FaceTarget() {
        transform.position = transform.parent.position;
        Vector3 targetPosition = target.transform.position;
        Quaternion lookDirection = Quaternion.LookRotation(targetPosition - transform.position);
        // Might want to consider adding additional guesses to the rotation to speed convergence
        lookDirection = Quaternion.Euler(lookDirection.eulerAngles.x, lookDirection.eulerAngles.y, lookDirection.eulerAngles.z);
        optimalXRotation = lookDirection.eulerAngles.x;
        optimalYRotation = lookDirection.eulerAngles.y;
        
        // Remove any extra rotations, since we only want to rotate in the X & Y direction
        lookDirection.z = 0f;

        if (transform.rotation != lookDirection) {
            transform.rotation = lookDirection;
        }
    }

    void FireTest() {
        if (testFire) {
            transform.position = initialPosition;
            transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x + optimalXRotation, initialRotation.eulerAngles.y + optimalYRotation, initialRotation.eulerAngles.z);
            optimalRotation = transform.rotation;
            arrowController.FireArrow(1f);
            testFire = false;
        }
    }

    public void CalculateOptimalArrowPath() {
        // Start by initially facing the target, and recording the new position/rotation
        FaceTarget();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        iterationCount = 0;
        xRotationConverged = false;
        yRotationConverged = false;
        optimalXRotation = 0f;
        optimalYRotation = 0f;
        testFire = true;
        simulationRunning = true;
    }

    public Quaternion GetOptimalRotation() {
        return optimalRotation;
    }

    public bool GetSimulationRunning() {
        return simulationRunning;
    }
}
