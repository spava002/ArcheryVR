using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReticlePersistence : MonoBehaviour {
    [SerializeField] GameObject reticle;
    XRInteractorLineVisual[] xRInteractorLineVisuals;
    
    void Start() {
        xRInteractorLineVisuals = GetComponents<XRInteractorLineVisual>();
    }

    void Update() {
        foreach (XRInteractorLineVisual xRInteractorLineVisual in xRInteractorLineVisuals) {
            if (!xRInteractorLineVisual.reticle) {
                xRInteractorLineVisual.reticle = reticle;
            }
        }
    }
}
