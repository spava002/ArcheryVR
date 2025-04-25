using TMPro;
using UnityEditor;
using UnityEngine;

public class DisplayPlayerStats : MonoBehaviour {
    TMP_Text playerStatsText;
    PointsManager pointsManager;

    int totalPointsEarned;
    float accuracyScore;
    float averagePointsPerHit;

    void Awake() {
        playerStatsText = GetComponentInChildren<TMP_Text>();
        pointsManager = FindAnyObjectByType<PointsManager>();    
    }

    void OnEnable() {
        CalculateAndDisplayResults();
    }

    void CalculateAndDisplayResults() {
        // Future proof by running through the whole player list in case more than 1 player can be in a match
        PointsManager.Player[] players = pointsManager.GetPlayers();

        playerStatsText.text = "";
        foreach (PointsManager.Player player in players) {
            totalPointsEarned = player.GetPoints();
            accuracyScore = (float)player.GetTotalHits() / player.GetTotalFires() * 100f;
            if (float.IsNaN(accuracyScore)) {
                accuracyScore = 0f;
            }

            if (player.GetTotalHits() != 0) {
                averagePointsPerHit = (float)player.GetPoints() / player.GetTotalHits();
            } 
            else {
                averagePointsPerHit = 0f;
            }

            playerStatsText.text += player.GetPlayerType() + " Stats\nTotal Points: " + totalPointsEarned + "\nAccuracy: " + accuracyScore.ToString("F2") + "%\nAverage Points Per Hit: " + averagePointsPerHit.ToString("F2") + "\n\n"; 
        }       
    }
}
