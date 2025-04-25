using System.Collections;
using UnityEngine;

public class CompassWindVisualizer : MonoBehaviour {
    [Tooltip("How fast the needle reaches its target rotation when updated.")]
    [SerializeField] float rotationSpeed;
    WindController windController;
    RectTransform rectTransform;
    // How similar the dot product of the needle's rotation vs the target rotation needs to be before we stop updating the rotation
    const float minSimilarityThreshold = 0.0001f;
    // Since the wind controller is built as West being 0 degrees, we need this slight offset on the y rotation, so that proper calculations are made
    const float yOffset = 90f;

    void OnEnable() {
        windController = FindAnyObjectByType<WindController>();
        rectTransform = GetComponent<RectTransform>();

        StartCoroutine(UpdateNeedleRotation(windController.GetWindAngle()));
    }

    public IEnumerator UpdateNeedleRotation(float targetAngle) {
        // Adding 180f to the targetAngle because its calculated via Vector3.SignedAngle, which return values between -180 and 180
        Quaternion targetRotation = Quaternion.Euler(new Vector3(rectTransform.rotation.eulerAngles.x, rectTransform.rotation.eulerAngles.y, yOffset - (targetAngle + 180f)));
        
        // Compute the similarity of the two angles, and once they are equal to close enough, snap it to the targetRotation
        while (Mathf.Abs(Quaternion.Dot(rectTransform.rotation, targetRotation)) < 1f - minSimilarityThreshold) {
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        rectTransform.rotation = targetRotation;
    }
}
