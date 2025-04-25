using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class InteractableButton :  XRSimpleInteractable {
    [Header("Manager")]
    [SerializeField] ButtonManager buttonManager;
    
    [Header("Audio")]
    [SerializeField] AudioClip buttonClick;
    [SerializeField] [Range(0f, 1f)] float volume;

    [Header("Colors")]
    [SerializeField] Color normalColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color pressedColor;
    [SerializeField] Color selectedColor;

    AudioSource audioSource;
    Image buttonImage;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        buttonImage = GetComponent<Image>();
        buttonImage.color = normalColor;
    }

    void Update() {
        // Placing here for now because it doesnt find it in Start method
        if (!interactionManager) {
            interactionManager = FindAnyObjectByType<XRInteractionManager>();
        }
    }

    // Overriding the XRSimpleInteractor's base interaction events
    protected override void OnHoverEntered(HoverEnterEventArgs args) {
        base.OnHoverEntered(args);
        buttonImage.color = highlightedColor;
    }

    protected override void OnHoverExited(HoverExitEventArgs args) {
        base.OnHoverExited(args);
        buttonImage.color = normalColor;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);
        buttonImage.color = pressedColor;
        if (audioSource) {
            // Might be good idea to replace 0.2f with a variable thats set in inspector
            audioSource.PlayOneShot(buttonClick, volume);
        }
        buttonManager.ApplyButtonAction(this);
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);
        buttonImage.color = selectedColor;
    }
}
