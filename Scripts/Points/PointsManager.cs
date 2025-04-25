using System;
using UnityEngine;

public class PointsManager : MonoBehaviour {
    MatchDisplay matchDisplay;
    Player[] players;
    int earnedPoints;
    bool isPractice;

    // Player class to store the points count
    // Future-proof as more players can be instantiated easily if a feature for it becomes feasible
    public class Player {
        PlayerType playerType;
        int points = 0;
        int totalHits = 0;
        int totalFires = 0;

        public Player(PlayerType playerType) {
            this.playerType = playerType;
        }

        public PlayerType GetPlayerType() {
            return playerType;
        }

        public int GetPoints() {
            return points;
        }

        public int GetTotalHits() {
            return totalHits;
        }

        public int GetTotalFires() {
            return totalFires;
        }

        public void IncreasePoints(int points) {
            this.points += points;
        }

        public void IncreaseTotalHits() {
            totalHits++;
        }

        public void IncreaseTotalFires() {
            totalFires++;
        }
    }

    void Awake() {
        isPractice = (DifficultyLevel)PlayerPrefs.GetInt("Difficulty") == DifficultyLevel.Practice;
        // Not ideal because intermatchcontroller is also calling this, but we need this initialized ASAP because ai controller relies on it
        InitializePointsManager();
    }

    public void InitializePointsManager() {
        earnedPoints = 0;
        PlayerType[] playerTypes;

        // If player has chosen the Practice difficulty, then we only create 1 Player
        if (isPractice) {
            Player newPlayer = new Player(PlayerType.Player1);
            players = new[] {newPlayer};
        }
        // Any other difficulty, then we create a Player for every PlayerType except 'None'
        else {
            playerTypes = (PlayerType[])Enum.GetValues(typeof(PlayerType));
            // Length - 1 because we aren't including the 'None' PlayerType as a player
            players = new Player[playerTypes.Length - 1];

            // Start at 1 instead of 0, so we skip the 'None' PlayerType
            for (int i = 1; i < playerTypes.Length; i++) {
                Player newPlayer = new Player(playerTypes[i]);
                players[i - 1] = newPlayer;
            }
        }

        if (matchDisplay) {
            matchDisplay.UpdateAndDisplayPlayerInfo();
        }
    }

    public void IncreasePoints(int increaseAmount, PlayerType arrowOwner) {
        foreach(Player player in players) {
            if (player.GetPlayerType() == arrowOwner) {
                player.IncreaseTotalFires();
                // Arrow controller may sometimes give an increase amount of 0, so we can display that 0 points were earned (indicating a miss)
                // We don't want to increase the total hits if 0 points were earned, because total hits represent hits on the target itself
                if (increaseAmount > 0) {
                    player.IncreasePoints(increaseAmount);
                    player.IncreaseTotalHits();
                }
            }
        }
        // Earned points simply display how many points the recent arrow hit earned
        earnedPoints = increaseAmount;

        if (!matchDisplay) {
            matchDisplay = FindAnyObjectByType<MatchDisplay>();
        }
        matchDisplay.UpdateAndDisplayPlayerInfo();
    }

    public Player[] GetPlayers() {
        return players;
    }

    public int GetEarnedPoints() {
        return earnedPoints;
    }
}
