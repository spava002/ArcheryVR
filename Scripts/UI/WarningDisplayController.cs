using System.Collections;
using UnityEngine;

public class WarningDisplayController : MonoBehaviour {
    [SerializeField] GameObject warningDisplay;

    [Tooltip("The amount of time that the warning stays active for.")]
    [SerializeField] float warningDisplayTime;

    float elapsedTime = 0f;
    bool warningBeingDisplayed;

    public void DisplayOrResetWarning() {
        if (warningBeingDisplayed) {
            elapsedTime = 0f;
        }
        else {
            StartCoroutine(DisplayWarning());
        }
    }

    public IEnumerator DisplayWarning() {
        warningDisplay.SetActive(true);
        warningBeingDisplayed = true;
        elapsedTime = 0f;
        while (elapsedTime < warningDisplayTime) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (elapsedTime >= warningDisplayTime) {
            warningDisplay.SetActive(false);
            warningBeingDisplayed = false;
        }
    }
}
