using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InitialGameSetup : MonoBehaviour {
    const string grabLayerMask = "Grab";
    XRDirectInteractor[] xRDirectInteractors;
    SpawnTarget targetSpawn;

    void Awake() {
        // USED FOR TESTING PURPOSES
        // PlayerPrefs.SetInt("Difficulty", (int)DifficultyLevel.Easy);
        // PlayerPrefs.SetInt("Mode", (int)ModeType.Bo5);
        // PlayerPrefs.SetInt("TargetDistance", (int)TargetDistance.EasyRange);
        // PlayerPrefs.SetInt("WindDifficulty", (int)WindDifficulty.EasyWind);

        // Debug.Log((DifficultyLevel)PlayerPrefs.GetInt("Difficulty"));
        // Debug.Log((ModeType)PlayerPrefs.GetInt("Mode"));
        // Debug.Log((TargetDistance)PlayerPrefs.GetInt("TargetDistance"));
        // Debug.Log((WindDifficulty)PlayerPrefs.GetInt("WindDifficulty"));

        // Toggle the quiver's spawn arrow script to enable it
        FindAnyObjectByType<QuiverManager>().ToggleSpawnArrow();

        // This is only necessary when loading back into another match
        // Because when we exit a match, the grab functionality is lost, so we need to enable it when joining a new match
        FindAndSetXRDirectInteractorsMask(grabLayerMask);

        ToggleTargetsAndOpponent();

        targetSpawn = FindAnyObjectByType<SpawnTarget>();
        targetSpawn.InitializeTargetDistance();
    }

    void ToggleTargetsAndOpponent() {
        // If not playing on practice difficulty, then disable all of the practice targets
        // Reasoning is because the opponent AI functions only with 1 target active, and we only want 1 target to be the focus of the game
        if ((DifficultyLevel)PlayerPrefs.GetInt("Difficulty") != DifficultyLevel.Practice) {
            GameObject[] practiceTargets = GameObject.FindGameObjectsWithTag("PracticeTarget");
            foreach (GameObject practiceTarget in practiceTargets) {
                practiceTarget.SetActive(false);
            }
        }
        // If playing on practice difficulty, then disable the opponent AI, so the player can play alone
        else {
            GameObject opponentAI = FindAnyObjectByType<AIController>().gameObject;
            opponentAI.SetActive(false);
        }
    }

    void FindAndSetXRDirectInteractorsMask(string mask) {
        if (xRDirectInteractors == null) {
            xRDirectInteractors = FindObjectsByType<XRDirectInteractor>(FindObjectsSortMode.None);
        }
        
        foreach (XRDirectInteractor xRDirectInteractor in xRDirectInteractors) {
            xRDirectInteractor.interactionLayers = InteractionLayerMask.GetMask(mask);
        }
    }
}
