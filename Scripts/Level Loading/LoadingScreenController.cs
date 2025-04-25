using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour {
    [Header("Progress Visualizers")]
    [SerializeField] Slider progressBar;
    [SerializeField] TMP_Text progressText;

    [SerializeField] TMP_Text informationText;

    LevelLoader levelLoader;

    void Start() {
        levelLoader = FindAnyObjectByType<LevelLoader>();

        levelLoader.LoadChosenLevel();

        informationText.text = "Now Loading: " + PlayerPrefs.GetString("Map") + "..."; 
    }

    void Update() {
        float loadProgress = levelLoader.GetLoadProgress();

        // Display progress on the progress bar
        progressBar.value = loadProgress;
        // Display progress as text
        // Convert progress to 0-100% range
        progressText.text = Mathf.Round(loadProgress * 10000) / 100 + "%/100%"; 
    }
}
