using UnityEngine;

public class BowstringRender : MonoBehaviour {
    LineRenderer lineRenderer;

    [SerializeField] int totalPoints;
    [SerializeField] GameObject anchorPoint1;
    [SerializeField] GameObject anchorPoint2;
    [SerializeField] GameObject intermediatePoint;

    void Start() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = totalPoints;
    }

    void Update() {
        lineRenderer.SetPosition(0, anchorPoint1.transform.position);
        lineRenderer.SetPosition(1, intermediatePoint.transform.position);
        lineRenderer.SetPosition(2, anchorPoint2.transform.position);
    }
}
