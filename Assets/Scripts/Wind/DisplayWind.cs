using TMPro;
using UnityEngine;

public class DisplayWind : MonoBehaviour {
    WindController windController;
    TextMeshPro windText;

    void Start() {
        windController = FindAnyObjectByType<WindController>();
        windText = GetComponentInChildren<TextMeshPro>();
    }

    void Update() {
        // Truncates windspeed to only 2 decimal places
        float truncatedWindspeed = (int)(windController.GetWindspeed() * 100f) / 100f;
        
        windText.text = "Windspeed: " + truncatedWindspeed + "\nDirection: " + windController.GetCardinalWindDirection();
    }
}
