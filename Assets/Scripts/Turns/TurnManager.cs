using System;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    [SerializeField] int turnsPerMatch;
    [SerializeField] float stoppingDistance;

    PlayerType[] playerTypes;
    // [SerializeField] PlayerType playerTurn;
    PlayerType playerTurn;
    int turnCount = 0;

    // void Start() {
    //     StartGame();
    // }

    void IncrementTurn() {
        int nextTurnIndex = (Array.IndexOf(playerTypes, playerTurn) + 1) % playerTypes.Length;
        // Skip over the index where the player type = None
        if (playerTypes[nextTurnIndex] == PlayerType.None) {
            nextTurnIndex++;
        }
        playerTurn = playerTypes[nextTurnIndex];
    }

    void EndGame() {
        playerTurn = PlayerType.None;
    }

    public void StartGame() {
        playerTypes = (PlayerType[])Enum.GetValues(typeof(PlayerType));

        // Randomly decide which player gets the first turn
        int initialTurn = UnityEngine.Random.Range(1, playerTypes.Length);
        playerTurn = playerTypes[initialTurn];
    }

    public void UpdateTurn() {
        if (turnCount != turnsPerMatch) {
            IncrementTurn();
            turnCount++;
        }
        else {
            EndGame();
        }
    }

    // Here we stop the arrow's movement if it has been fired out of turn
    // We only do this for the player, because the AI isnt capable of firing out of turn by design
    public bool CheckIfFiringOutOfTurn(PlayerType arrowOwner) {
        return playerTurn != arrowOwner;
    }

    public PlayerType GetTurn() {
        return playerTurn;
    }
}
