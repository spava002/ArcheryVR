using TMPro;
using UnityEngine;

// In charge of displaying scores, current turn, amount of turns left, and wind info
public class MatchDisplay : MonoBehaviour {
    PlayerType turn;
    int remainingTurnsCount;
    PointsManager.Player[] players;
    int earnedPoints;
    float windSpeed;
    string cardinalWindDirection;

    TurnManager turnManager;
    PointsManager pointsManager;
    WindController windController;
    TMP_Text ongoingMatchText;

    void Awake() {
        turnManager = FindAnyObjectByType<TurnManager>();
        pointsManager = FindAnyObjectByType<PointsManager>();
        windController = FindAnyObjectByType<WindController>();

        ongoingMatchText = GetComponentInChildren<TMP_Text>();    
    }

    void OnEnable() {
        InitializeDisplayInfo();
    }

    void DisplayMatchInfo() {
        // Turn info
        ongoingMatchText.text = "Current Turn: " + turn + "\n";
        // Only applies to practice matches, where turns are infinite for the player
        if (remainingTurnsCount == int.MaxValue) {
            ongoingMatchText.text += "Turns Remaining: -" + "\n\n";
        }
        else {
            ongoingMatchText.text += "Turns Remaining: " + remainingTurnsCount + "\n\n";
        }

        // Points info
        ongoingMatchText.text += "Total Points\n";
        foreach(PointsManager.Player player in players) {
            ongoingMatchText.text += player.GetPlayerType() + ": " + player.GetPoints() + "\n";
        }
        ongoingMatchText.text += "Recent Shot Earned: " + earnedPoints + "\n\n";

        // Wind Info
        ongoingMatchText.text += "Windspeed: " + windSpeed + " mph\nDirection: " + cardinalWindDirection;
    }

    void InitializeDisplayInfo() {
        turn = turnManager.GetTurn();
        remainingTurnsCount = turnManager.GetRemainingTurns();
        players = pointsManager.GetPlayers();
        earnedPoints = pointsManager.GetEarnedPoints();
        windSpeed = windController.GetTruncatedWindspeed();
        cardinalWindDirection = windController.GetCardinalWindDirection();

        DisplayMatchInfo();
    }

    public void UpdateAndDisplayTurnInfo() {
        turn = turnManager.GetTurn();
        remainingTurnsCount = turnManager.GetRemainingTurns();
        DisplayMatchInfo();
    }

    public void UpdateAndDisplayPlayerInfo() {
        players = pointsManager.GetPlayers();
        earnedPoints = pointsManager.GetEarnedPoints();
        DisplayMatchInfo();
    }

    public void UpdateAndDisplayWindInfo() {
        windSpeed = windController.GetTruncatedWindspeed();
        cardinalWindDirection = windController.GetCardinalWindDirection();
        DisplayMatchInfo();
    }
}
