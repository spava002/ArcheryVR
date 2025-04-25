using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelFadeController : MonoBehaviour {
    [SerializeField] float fadeTime;

    Image blackFade;
    const int maxAlphaValue = 1;

    void Start() {
        blackFade = GetComponentInChildren<Image>();
    }

    public void InitiateScreenFade() {
        StartCoroutine(ScreenFade());
    }

    IEnumerator ScreenFade() {
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            float fadePercentage = elapsedTime / fadeTime;
            blackFade.color = new Color(blackFade.color.r, blackFade.color.g, blackFade.color.b, maxAlphaValue * fadePercentage);
            yield return null;
        }

        yield return new WaitForSeconds(fadeTime);
        elapsedTime = 0f;

        while (elapsedTime < fadeTime) {
            elapsedTime += Time.deltaTime;
            float fadePercentage = elapsedTime / fadeTime;
            blackFade.color = new Color(blackFade.color.r, blackFade.color.g, blackFade.color.b, maxAlphaValue * (1 - fadePercentage));
            yield return null;
        }
    }

    public float GetFadeTime() {
        return fadeTime;
    }
}
