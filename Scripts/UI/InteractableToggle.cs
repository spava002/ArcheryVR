using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class InteractableToggle :  XRSimpleInteractable {
    [Header("Manager")]
    [SerializeField] ToggleManager toggleManager;
    [Header("Audio")]
    [SerializeField] AudioClip buttonClick;

    AudioSource audioSource;
    Toggle toggle;
    
    void Start() { 
        audioSource = GetComponent<AudioSource>();
        toggle = GetComponent<Toggle>();
    }

    void Update() {
        if (!interactionManager) {
            interactionManager = FindAnyObjectByType<XRInteractionManager>();
        }    
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);

        audioSource.PlayOneShot(buttonClick, 0.25f);

        // Check if any toggles are currently on, and if so, turn them off
        toggleManager.DisableAnyActiveToggles();
        // Turn this toggle on now
        toggle.isOn = !toggle.isOn;
        // Store the value as a player pref
        toggleManager.SetPlayerPref(toggle);
    }
}