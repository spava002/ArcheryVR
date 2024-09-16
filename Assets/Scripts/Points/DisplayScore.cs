using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour {
    PointsManager pointsManager;
    TextMeshPro pointsText;

    void Start() {
        pointsManager = FindAnyObjectByType<PointsManager>();
        pointsText = GetComponentInChildren<TextMeshPro>();
    }

    void Update() {
        PointsManager.Player[] players = pointsManager.GetPlayers();
        pointsText.text = "Total Points:\n";
        foreach(PointsManager.Player player in players) {
            pointsText.text += player.playerType + ": " + player.points + "\n";
        }
        pointsText.text += "\nEarned: " + pointsManager.GetEarnedPoints();
    }
}
