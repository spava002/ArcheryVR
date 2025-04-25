using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class InterMatchController : MonoBehaviour {
    [SerializeField] GameObject postMatchUI;

    XRDirectInteractor[] xRDirectInteractors;
    XRRayInteractor[] xRRayInteractors;
    MatchInformationControl matchInformationControl;
    PointsManager pointsManager;
    WindController windController;
    GameObject playerBow;
    Vector3 playerBowInitialPosition;
    Quaternion playerBowInitialRotation;
    const string noneLayerMask = "None";
    const string grabLayerMask = "Grab";
    bool rematch = false;
    
    void Awake() {
        xRDirectInteractors = FindObjectsByType<XRDirectInteractor>(FindObjectsSortMode.None);
        xRRayInteractors = FindObjectsByType<XRRayInteractor>(FindObjectsSortMode.None);
        matchInformationControl = FindAnyObjectByType<MatchInformationControl>();
        pointsManager = FindAnyObjectByType<PointsManager>();
        windController = FindAnyObjectByType<WindController>();

        playerBow = GameObject.FindWithTag("PlayerBow");
        playerBowInitialPosition = playerBow.transform.position;
        playerBowInitialRotation = playerBow.transform.rotation;

        // Turns off the ray interactors
        ToggleXRRayInteractors();
    }

    // Called when the game starts (bow picked up by player)
    public void InitiatePreMatchSequence() {
        matchInformationControl.ToggleMatchDisplays();
        if (!rematch) {
            DisablePostMatchUI();
            InitializeXRInteractors();
            rematch = true;
        }
    }

    // Called when the game ends
    public void InitiatePostMatchSequence() {
        matchInformationControl.ToggleMatchDisplays();
        TogglePostMatchUI();
        ToggleXRRayInteractors();
        SetXRDirectInteractorsMask(noneLayerMask);
        ResetBowPositionAndRotation();
    }

    // Called when the rematch button on the post match UI is pressed
    public void InitiateIntermissionMatchSequence() {
        // Enable the intermission match display
        matchInformationControl.ToggleMatchDisplays();
        TogglePostMatchUI();
        ToggleXRRayInteractors();
        SetXRDirectInteractorsMask(grabLayerMask);
        pointsManager.InitializePointsManager();
        windController.InitializeWindController();
    }

    void ToggleXRRayInteractors() {
        foreach (XRRayInteractor xRRayInteractor in xRRayInteractors) {
            xRRayInteractor.gameObject.SetActive(!xRRayInteractor.gameObject.activeSelf);
        }
    }

    void SetXRDirectInteractorsMask(string mask) {
        foreach (XRDirectInteractor xRDirectInteractor in xRDirectInteractors) {
            xRDirectInteractor.interactionLayers = InteractionLayerMask.GetMask(mask);
        }
    }

    void InitializeXRInteractors() {
        foreach (XRRayInteractor xRRayInteractor in xRRayInteractors) {
            xRRayInteractor.gameObject.SetActive(false);
        }

        SetXRDirectInteractorsMask(grabLayerMask);
    }

    void ResetBowPositionAndRotation() {
        playerBow.transform.position = playerBowInitialPosition;
        playerBow.transform.rotation = playerBowInitialRotation;
    }

    void DisablePostMatchUI() {
        postMatchUI.SetActive(false);
    }

    void TogglePostMatchUI() {
        postMatchUI.SetActive(!postMatchUI.activeSelf);
    }
}
