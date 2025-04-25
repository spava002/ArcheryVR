using UnityEngine;

public class InitialPlayerSettings : MonoBehaviour {
    void Start () {
        // Set default PlayerPref settings
        PlayerPrefs.SetString("Map", "Training Fields");
        PlayerPrefs.SetInt("Difficulty", (int)DifficultyLevel.Easy);
        PlayerPrefs.SetInt("Mode", (int)ModeType.Bo7);
        PlayerPrefs.SetInt("TargetDistance", (int)TargetDistance.EasyRange);
        PlayerPrefs.SetInt("WindDifficulty", (int)WindDifficulty.EasyWind);
    }
}
