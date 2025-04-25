using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour {
    [Header("Fade Times")]
    [Tooltip("Amount of time time it takes the music to fade in/out to the min and max volumes.")]
    [SerializeField] float initialFadeInTime;
    [SerializeField] float defaultFadeTime;

    [Header("Volume Levels")]
    [SerializeField] float maxPlayingVolume;
    [SerializeField] float minPlayingVolume;

    [Tooltip("Place in order of Main Menu, and then followed by the map order displayed on the maps menu.")]
    [System.Serializable]
    public class Map {
        [SerializeField] string name;
        [SerializeField] AudioClip audioClip;

        public string GetName() {
            return name;
        }

        public AudioClip GetAudioClip() {
            return audioClip;
        }
    }

    [SerializeField] Map[] maps;

    AudioSource audioSource;
    const string mainMenu = "Main Menu";
    const float volumeOff = 0f;
    bool initialFadeIn = true;
    bool loadFadeCycle;
    int loadFadeCycleCount;

    void Start() {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = maps[0].GetAudioClip();
        audioSource.volume = 0f;
        audioSource.Play();

        InitiateMusicFade();
    }

    // Small bug where if the player clicks start before the initial fade in has finished, the volume stays at maxPlayingVolume (it skips over part of the fade cycle)
    // Temporary fix is to just have the initial fade in time to be relatively short, so it finishes before that
    public void InitiateMusicFade() {
        if (initialFadeIn) {
            StartCoroutine(MusicFade(initialFadeInTime, maxPlayingVolume));
        }
        else {
            if (!loadFadeCycle) {
                StartCoroutine(MusicFade(defaultFadeTime, minPlayingVolume));
            }
            else {
                StartCoroutine(MusicFade(defaultFadeTime, volumeOff));
            }
        }
    }

    // Target volume defines the volume that the fade out will go to, fade in volume is always maxPlayingVolume
    IEnumerator MusicFade(float fadeTime, float targetVolume) {
        float currentVolume = audioSource.volume;
        float volumeChangeRate;
        float elapsedTime = 0;
        // Turn up the volume from 0 to maxPlayingVolume with this fade in
        if (initialFadeIn) {
            // Initial fade in
            // Debug.Log("Initial Fade In");
            while (elapsedTime < fadeTime) {
                elapsedTime += Time.deltaTime;
                volumeChangeRate = elapsedTime / fadeTime;
                audioSource.volume = Mathf.Lerp(currentVolume, targetVolume, volumeChangeRate);
                yield return null;
                audioSource.volume = targetVolume;
            }

            initialFadeIn = false;
        }
        else {
            // Debug.Log("Load Fade Cycle: " + loadFadeCycle);
            // Fade out
            // Debug.Log("Fade Out");
            while (elapsedTime < fadeTime) {
                elapsedTime += Time.deltaTime;
                volumeChangeRate = elapsedTime / fadeTime;
                audioSource.volume = Mathf.Lerp(currentVolume, targetVolume, volumeChangeRate);
                yield return null;
                audioSource.volume = targetVolume;
            }

            // Need to load new song here, after one fade cycle has completed
            if (loadFadeCycle) {
                // Debug.Log("Its a Fade Cycle. Loading level's song.");
                audioSource.Stop();
                // City ambience, need to make dynamic based on level
                if (loadFadeCycleCount == 0) {
                    // Compare chosen map to the map names we have saved in Maps class, and load the corresponding audio clip
                    foreach (Map map in maps) {
                        if (PlayerPrefs.GetString("Map") == map.GetName()) {
                            // Debug.Log("Loading Audio Clip For: " + PlayerPrefs.GetString("Map"));
                            audioSource.clip = map.GetAudioClip();
                        }
                    }
                }
                else {
                    // Debug.Log("Loading Audio Clip For: " + maps[0].GetName());
                    audioSource.clip = maps[0].GetAudioClip();
                }
                audioSource.Play();
                loadFadeCycleCount++;
            }
            else {
                yield return new WaitForSeconds(fadeTime);
            }

            targetVolume = maxPlayingVolume;
            currentVolume = audioSource.volume;
            elapsedTime = 0f;

            // Fade in
            // Debug.Log("Fade In");
            while (elapsedTime < fadeTime) {
                elapsedTime += Time.deltaTime;
                volumeChangeRate = elapsedTime / fadeTime;
                audioSource.volume = Mathf.Lerp(currentVolume, targetVolume, volumeChangeRate);
                yield return null;
                audioSource.volume = targetVolume;
            }


            if (loadFadeCycleCount == 2) {
                loadFadeCycle = false;
                loadFadeCycleCount = 0;
            }
            // If we load into the main menu, we don't want to set the next fade cycle to be a load fade cycle
            // Fade cycles only occur when going from the transition scene to the selected level, OR when we are going from the selected level to the transition screen
            else if (!loadFadeCycle && SceneManager.GetActiveScene().name != mainMenu) {
                // Debug.Log("Fade Cycle Set to True");
                loadFadeCycle = true;
            }
        }
    }
}
