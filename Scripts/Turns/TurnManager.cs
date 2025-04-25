using System;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    [SerializeField] GameObject exitMatchButton;

    int turnsPerPlayer;

    InterMatchController interMatchController;
    MatchDisplay matchDisplay;
    PlayerType[] playerTypes;
    PlayerType playerTurn;
    int turnCount;
    bool isPractice;

    void Start() {
        interMatchController = FindAnyObjectByType<InterMatchController>();

        isPractice = (DifficultyLevel)PlayerPrefs.GetInt("Difficulty") == DifficultyLevel.Practice;
        // Used for testing purposes, to override having to pick up the bow to commence the game
        // StartGame();
    }

    void InitializeTurnsPerMatch() {
        int amountOfPlayers = playerTypes.Length - 1;
        ModeType mode = (ModeType)PlayerPrefs.GetInt("Mode");
        switch (mode) {
            // Mode type of 'None' signifies its a practice match, so we give the player an 'infinite' amount of turns
            case ModeType.None:
                turnsPerPlayer = int.MaxValue;
                break;
            // Multiplied by 2 in this case, because we have that many players playing that many turns per match
            case ModeType.Bo5:
                turnsPerPlayer = 5 * amountOfPlayers;
                break;
            case ModeType.Bo7:
                turnsPerPlayer = 7 * amountOfPlayers;
                break;
            case ModeType.Bo9:
                turnsPerPlayer = 9 * amountOfPlayers;
                break;
        }
    }

    void IncrementTurn() {
        int nextTurnIndex = (Array.IndexOf(playerTypes, playerTurn) + 1) % playerTypes.Length;
        // Skip over the index where the player type = None
        if (playerTypes[nextTurnIndex] == PlayerType.None) {
            nextTurnIndex++;
        }
        playerTurn = playerTypes[nextTurnIndex];

        if (!matchDisplay) {
            matchDisplay = FindAnyObjectByType<MatchDisplay>();
        }
        matchDisplay.UpdateAndDisplayTurnInfo();
    }

    // Public because can be called by the exit match button
    public void EndGame() {
        playerTurn = PlayerType.None;
        interMatchController.InitiatePostMatchSequence();

        exitMatchButton.SetActive(false);
    }

    public void StartGame() {
        turnCount = 0;

        // Get all of the players
        if (isPractice) {
            playerTypes = new[] {PlayerType.Player1};

            playerTurn = playerTypes[0];
        }
        else {
            playerTypes = (PlayerType[])Enum.GetValues(typeof(PlayerType));

            // Randomly decide which player gets the first turn, skip first index because index 0 signifies 'None' PlayerType
            int initialTurn = UnityEngine.Random.Range(1, playerTypes.Length);
            playerTurn = playerTypes[initialTurn];
        }

        InitializeTurnsPerMatch();

        if (matchDisplay) {
            matchDisplay.UpdateAndDisplayTurnInfo();
        }

        interMatchController.InitiatePreMatchSequence();

        exitMatchButton.SetActive(true);
    }

    public void UpdateTurn() {
        if (turnCount != turnsPerPlayer) {
            turnCount++;
            IncrementTurn();
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

    // Used to return turnCount, incase anything breaks
    public int GetTurnsPerPlayer() {
        return turnsPerPlayer / (playerTypes.Length - 1);
    }

    public int GetRemainingTurns() {
        if (isPractice) {
            return turnsPerPlayer;
        }

        return Mathf.CeilToInt((float)(turnsPerPlayer - turnCount) / (playerTypes.Length - 1));
    }
}
