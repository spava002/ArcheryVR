using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowInteractable : XRGrabInteractable {
    TurnManager turnManager;

    void Start() {
        turnManager = FindAnyObjectByType<TurnManager>();
        selectEntered.AddListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args) {
        // Only start a fresh game if the turn hasn't been set to a player yet ('None' being no player)
        if (turnManager.GetTurn() == PlayerType.None) {
            turnManager.StartGame();
        }
    }
}
