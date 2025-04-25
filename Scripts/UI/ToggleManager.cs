using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour {
    public enum ToggleType {
        Mode,
        Difficulty
    }

    [SerializeField] ToggleType toggleType;
    [SerializeField] Toggle[] toggles;

    [Header("Menus")]
    [Tooltip("Modes menu in main menu thats disabled when player chooses Practice difficulty. Only necessary when toggleType is on Difficulty.")]
    [SerializeField] GameObject modesMenu;

    ModeType previouslySetMode;

    // void Update() {
    //     Debug.Log((ModeType)PlayerPrefs.GetInt("Mode"));    
    //     Debug.Log((DifficultyLevel)PlayerPrefs.GetInt("Difficulty"));  
    // }

    public void DisableAnyActiveToggles() {
        foreach (Toggle toggle in toggles) {
            if (toggle.isOn) {
                toggle.isOn = false;
            }
        }
    }

    public void SetPlayerPref(Toggle toggle) {
        int i = 0; 
        // Find the button that has been pressed and save its index
        foreach (Toggle currToggle in toggles) {
            if (currToggle == toggle) {
                break;
            }
            i++;
        }

        if (toggleType == ToggleType.Mode) {
            ModeType modeType = (ModeType)(i + 1);
            PlayerPrefs.SetInt("Mode", (int)modeType);
        }
        else if (toggleType == ToggleType.Difficulty) {
            DifficultyLevel difficultyLevel = (DifficultyLevel)i;
            PlayerPrefs.SetInt("Difficulty", (int)difficultyLevel);

            TargetDistance targetDistance = (TargetDistance)i;
            PlayerPrefs.SetInt("TargetDistance", (int)targetDistance);

            WindDifficulty windDifficulty = (WindDifficulty)i;
            PlayerPrefs.SetInt("WindDifficulty", (int)windDifficulty);

            // Disable the mode menu found in the main menu if player chooses a Practice difficulty
            // Modes are not available on practice games
            if (difficultyLevel == DifficultyLevel.Practice) {
                // Saves the previously selected ModeType, incase the player chooses to switch out of practice difficulty, we can revert their selected mode
                previouslySetMode = (ModeType)PlayerPrefs.GetInt("Mode");
                // Set mode to 'None', since Practice difficulty cant have any modes
                PlayerPrefs.SetInt("Mode", (int)ModeType.None);
                modesMenu.SetActive(false);
            }
            else {
                if (previouslySetMode != ModeType.None) {
                    // Loads the previously selected ModeType, if player switches back onto a difficulty that isn't Practice
                    PlayerPrefs.SetInt("Mode", (int)previouslySetMode);
                }
                modesMenu.SetActive(true);
            }
        }
    }
}
