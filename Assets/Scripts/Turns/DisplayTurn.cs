using System.Collections;
using TMPro;
using UnityEngine;

public class DisplayTurn : MonoBehaviour {
    // Might make more sense to place this somewhere else
    [SerializeField] float matchIntermissionTime;

    TurnManager turnManager;
    TextMeshPro turnText;
    PlayerType previousTurn;
    bool matchIntermssion;

    void Start() {
        turnManager = FindAnyObjectByType<TurnManager>();
        turnText = GetComponentInChildren<TextMeshPro>();
    }

    void Update() {
        PlayerType currentTurn = turnManager.GetTurn();
        if (!matchIntermssion) 
            if (previousTurn != PlayerType.None && currentTurn == PlayerType.None) {
                turnText.text = "Match Intermission...";
                matchIntermssion = true;
                StartCoroutine(RunMatchIntermission());
            }
            else if (previousTurn == PlayerType.None && currentTurn == PlayerType.None) {
                turnText.text = "Pick up the bow\nto start a match!";
            }
            else {
                turnText.text = "Current Turn:\n" + currentTurn;
            }
            previousTurn = currentTurn;
    }

    IEnumerator RunMatchIntermission() {
        yield return new WaitForSeconds(matchIntermissionTime);
        matchIntermssion = false;
    }
}
