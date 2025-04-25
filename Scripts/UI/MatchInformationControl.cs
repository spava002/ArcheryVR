using System;
using TMPro;
using UnityEngine;

public class MatchInformationControl : MonoBehaviour {
    [Tooltip("The displays that will be toggled throughout the duration of the match. Place in order of intermission, ongoing, and endgame displays for it to work properly.")]
    [SerializeField] GameObject[] matchDisplays;
    [Tooltip("The text that the header information text will be displaying at each display. Place in order of intermission, ongoing, and endgame displays for it to work.")]
    [SerializeField] string[] displayInformation;
    [SerializeField] TMP_Text displayInformationText;
    int displayToActivate;
    bool isPractice;

    void Start() {
        isPractice = (DifficultyLevel)PlayerPrefs.GetInt("Difficulty") == DifficultyLevel.Practice;

        InitializeDisplays();
    }

    void InitializeDisplays() {
        matchDisplays[0].SetActive(true);
        matchDisplays[1].SetActive(false);
        matchDisplays[2].SetActive(false);

        if (isPractice) {
            displayInformationText.text = displayInformation[3];
        }
        else {
            displayInformationText.text = displayInformation[0];
        }
    }

    public void ToggleMatchDisplays() {
        displayToActivate = (displayToActivate + 1) % matchDisplays.Length;
        for (int i = 0; i < matchDisplays.Length; i++) {
            if (i == displayToActivate) {
                matchDisplays[i].SetActive(true);
                // If in a practice match, display the practice text info instead
                if (isPractice) {
                    displayInformationText.text = displayInformation[displayInformation.Length / 2 + i];
                }
                else {
                    displayInformationText.text = displayInformation[i];
                }
            }
            else {
                matchDisplays[i].SetActive(false);
            }
        }
    }
}
