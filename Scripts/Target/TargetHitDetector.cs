using Unity.XR.CoreUtils;
using UnityEngine;

public class TargetHitDetector : MonoBehaviour {
    [SerializeField] float radius;
    [SerializeField] int sections;
    [SerializeField] int maxPointsToAward;

    PointsManager pointsManager;
    float sectionRadius;

    void Start() {
        pointsManager = FindAnyObjectByType<PointsManager>();
        sectionRadius = radius / sections;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Arrow")) {
            // Need to refine this part a bit
            // Basically, collision.point was giving in-accuracies, so we are using an empty gameobject at the arrow tip now, which is much more accurate for calculating distance

            PlayerType arrowOwner = other.gameObject.GetComponent<ArrowController>().GetArrowOwner();
            GameObject arrowTip = other.gameObject.GetNamedChild("Tip");

            Vector3 localNewCollisionPosition = transform.parent.InverseTransformPoint(arrowTip.transform.position);
            float newArrowDistanceToCenter = Vector3.Distance(localNewCollisionPosition, transform.localPosition);

            // Debug.Log(localNewCollisionPosition);
            // Debug.Log(transform.localPosition);
            // Debug.Log("Local Arrow Tip Calc: " + newArrowDistanceToCenter);

            // Get the first contact point, and calculate the distance to the center
            ContactPoint collision = other.GetContact(0);
            // Debug.Log(collision.point);
            // Debug.Log(transform.position);
            // Debug.Log("Global Calc: " + Vector3.Distance(collision.point, transform.position));
            Vector3 localCollisionPosition = transform.parent.InverseTransformPoint(collision.point);
            float arrowDistanceToCenter = Vector3.Distance(localCollisionPosition, transform.localPosition);
            // Debug.Log(localCollisionPosition);
            // Debug.Log(transform.localPosition);
            // Debug.Log("Local Collision Calc: " + arrowDistanceToCenter);

            CalculatePoints(newArrowDistanceToCenter, arrowOwner);
        }    
    }

    // Uses the distance to the center to calculate points
    void CalculatePoints(float arrowDistanceToCenter, PlayerType arrowOwner) {
        int pointsToAward = maxPointsToAward;
        for (int i = 1; i < sections + 1; i++) {
            if (arrowDistanceToCenter < sectionRadius * i) {
                pointsManager.IncreasePoints(pointsToAward, arrowOwner);
                break;
            }
            pointsToAward--;
        }
    }
}
