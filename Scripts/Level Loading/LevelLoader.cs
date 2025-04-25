using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    LevelFadeController levelFadeController;
    MusicPlayer musicPlayer;
    float loadWaitTime;
    const string transitionScene = "Transition Scene";
    float asyncLoadProgress;
    float timeLoadProgress;
    bool inTransitionScene;

    void Start() {
        levelFadeController = FindAnyObjectByType<LevelFadeController>();
        musicPlayer = FindAnyObjectByType<MusicPlayer>();
        // If loading is too fast, this acts as a way to delay the scene from loading until the amount of time has passed
        // By default this should always be twice the length of the fade time
        loadWaitTime = levelFadeController.GetFadeTime() * 2f;
    }

    public void LoadChosenLevel() {
        if (!inTransitionScene) {
            // Play fade effect
            if (!levelFadeController) {
                levelFadeController = FindAnyObjectByType<LevelFadeController>();
            }
            levelFadeController.InitiateScreenFade();

            if (!musicPlayer) {
                musicPlayer = FindAnyObjectByType<MusicPlayer>();
            }
            musicPlayer.InitiateMusicFade();
        }

        // Start loading the chosen level OR transition scene if currently not loaded
        StartCoroutine(LoadLevelAsynchronously());
    }

    IEnumerator LoadLevelAsynchronously() {
        AsyncOperation asyncLoad;
        if (inTransitionScene) {
            asyncLoad = SceneManager.LoadSceneAsync(PlayerPrefs.GetString("Map"), LoadSceneMode.Single);
        }
        else {
            asyncLoad = SceneManager.LoadSceneAsync(transitionScene, LoadSceneMode.Single);
        }
        asyncLoad.allowSceneActivation = false;

        float elapsedTime = 0f;

        while (!asyncLoad.isDone) {
            elapsedTime += Time.deltaTime;

            if (inTransitionScene) {
                asyncLoadProgress = asyncLoad.progress / 0.9f;
                timeLoadProgress = Mathf.Clamp01(elapsedTime / loadWaitTime);
            }

            if (asyncLoad.progress >= 0.9f && elapsedTime >= loadWaitTime) {
                if (inTransitionScene) {
                    // Play fade effect
                    levelFadeController.InitiateScreenFade();
                    musicPlayer.InitiateMusicFade();
                    // Wait until fade in has finished
                    yield return new WaitForSeconds(levelFadeController.GetFadeTime());
                }
                // Load the scene
                asyncLoad.allowSceneActivation = true;
                inTransitionScene = !inTransitionScene;
            }

            yield return null;
        }
    }

    // Return whichever is the smallest, since that reflects the actual loading time
    public float GetLoadProgress() {
        return Mathf.Min(asyncLoadProgress, timeLoadProgress);
    }
}
