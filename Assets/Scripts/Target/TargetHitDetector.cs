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
            PlayerType arrowOwner = other.gameObject.GetComponent<ArrowController>().GetArrowOwner();
            // Get the first contact point, and calculate the distance to the center
            ContactPoint collision = other.GetContact(0);
            Vector3 localCollisionPosition = transform.parent.InverseTransformPoint(collision.point);
            float arrowDistanceToCenter = Vector3.Distance(localCollisionPosition, transform.localPosition);
            CalculatePoints(arrowDistanceToCenter, arrowOwner);
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
