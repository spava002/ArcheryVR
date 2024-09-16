using System;
using UnityEngine;

public class PointsManager : MonoBehaviour {
    Player[] players;
    int earnedPoints = 0;

    // Player class to store the points count
    // Future-proof as more players can be instantiated easily if a feature for it becomes feasible
    public class Player {
        public PlayerType playerType;
        public int points = 0;
    }

    void Start() {
        PlayerType[] playerTypes = (PlayerType[])Enum.GetValues(typeof(PlayerType));
        // Length - 1 because we aren't including the 'None' PlayerType as a player
        players = new Player[playerTypes.Length - 1];

        // Start at 1 instead of 0, so we skip the 'None' PlayerType
        for (int i = 1; i < playerTypes.Length; i++) {
            Player newPlayer = new Player();
            newPlayer.playerType = playerTypes[i];
            players[i - 1] = newPlayer;
        }
    }

    public void IncreasePoints(int increaseAmount, PlayerType arrowOwner) {
        foreach(Player player in players) {
            if (player.playerType == arrowOwner) {
                player.points += increaseAmount;
            }
        }
        // Earned points simply display how many points the recent arrow hit earned
        earnedPoints = increaseAmount;
    }

    public Player[] GetPlayers() {
        return players;
    }

    public int GetEarnedPoints() {
        return earnedPoints;
    }
}
