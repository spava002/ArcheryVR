using UnityEngine;

public class TreeAnimationManager : MonoBehaviour {
    [Header("Animator Speed Ranges")]
    [Tooltip("The highest speed the animator can be set to.")]
    [SerializeField] float speedUpperRange;
    [Tooltip("The lowest speed the animator can be set to.")]
    [SerializeField] float speedLowerRange;

    [Tooltip("The maximum offset that a tree will randomly receive on its y-axis.")]
    [SerializeField] float yRotationOffset;

    readonly string[] animationTriggers = new string[] {"Light Breeze On", "Medium Breeze On", "Heavy Breeze On"};
    const int animationCount = 3;
    Animator[] treeAnimators;

    void ApplyYRotationOffset() {
        foreach (Animator treeAnimator in treeAnimators) {
            Quaternion treeRotation = treeAnimator.gameObject.transform.rotation;
            // Randomly add an offset using yRotationOffset to the tree's initial y rotation
            // This helps add a bit more variation between each tree's animation
            float newYRotation = treeRotation.eulerAngles.y + Random.Range(-yRotationOffset, yRotationOffset);
            Quaternion newRotation = Quaternion.Euler(new Vector3(treeRotation.x, newYRotation, treeRotation.z));

            // Set the new randomize rotation
            treeAnimator.gameObject.transform.rotation = newRotation;
        }
    }

    public void UpdateTreeAnimations(float windSpeed, float maxWindspeed) {
        int i;
        for (i = 1; i < animationCount + 1; i++) {
            if (windSpeed <= maxWindspeed * ((float)i / animationCount)) {
                break;
            }
        }

        // Debug.Log("Setting animation as: " + animationTriggers[i - 1]);

        if (treeAnimators == null) {
            // Save animator references in an array so that we can easily access them when updating the winds
            treeAnimators = GetComponentsInChildren<Animator>();
            ApplyYRotationOffset();
        }

        foreach (Animator animator in treeAnimators) {
            animator.SetTrigger(animationTriggers[i - 1]);
            // Small change in speech allows for a slight variation in each tree's animation
            animator.speed = Random.Range(speedLowerRange, speedUpperRange);
        }
    }
}
