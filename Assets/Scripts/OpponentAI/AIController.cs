using UnityEngine;
using UnityEngine.Animations;

public class AIController : MonoBehaviour {
    [SerializeField] float alignmentSpeed;

    ArrowPool arrowPool;
    ArrowPathOptimizer arrowPathOptimizer;
    TurnManager turnManager;
    GameObject arrow;
    ArrowController arrowController;
    bool alignedAndFired;

    void Start() {
        arrowPool = FindAnyObjectByType<ArrowPool>();
        arrowPathOptimizer = FindAnyObjectByType<ArrowPathOptimizer>();
        turnManager = FindAnyObjectByType<TurnManager>();
    }

    void Update() {
        if (arrow == null) {
            arrow = arrowPool.GetAvailableArrow();
            arrowController = arrow.GetComponent<ArrowController>();
            arrowController.SetArrowOwner(PlayerType.Player2);
            arrow.transform.position = transform.position;
            arrow.SetActive(true);
            arrowPathOptimizer.CalculateOptimalArrowPath();
            alignedAndFired = false;
        }

        // If more players are added down the line, a better practice would be to compare the GetTurn() to the last index of the enum, since it should always be the AI
        if (!arrowPathOptimizer.GetSimulationRunning() && !alignedAndFired && turnManager.GetTurn() == PlayerType.Player2) {
            // Rotation offets would be randomized here based on difficulties
            Quaternion optimalRotation = arrowPathOptimizer.GetOptimalRotation();
            arrow.transform.rotation = Quaternion.RotateTowards(arrow.transform.rotation, optimalRotation, alignmentSpeed * Time.deltaTime);
            if (Quaternion.Angle(arrow.transform.rotation, optimalRotation) <= Mathf.Epsilon) {
                // Since RotateTowards overshoots a little, and we need a precise rotation, we just rotate to the optimal rotation since we are close enough
                arrow.transform.rotation = optimalRotation;
                arrowController.FireArrow(1f);
                alignedAndFired = true;
            }
        }

        if (alignedAndFired != arrowController.GetIsMoving()) {
            arrow = null;
            arrowController = null;
        }
    }
}
