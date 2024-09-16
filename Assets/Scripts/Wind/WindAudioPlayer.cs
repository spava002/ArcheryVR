using UnityEngine;

public class WindAudioPlayer : MonoBehaviour {
    [SerializeField] float maxVolume;

    WindController windController;
    AudioSource audioSource;

    void Start() {
        windController = FindAnyObjectByType<WindController>();
        audioSource = GetComponent<AudioSource>();

        // Playing at the start will not always be the case
        // Need to re-visit once difficulties can be enabled, since wind wont be in all modes
        audioSource.Play();
    }

    // Update is called once per frame
    void Update() {
        audioSource.volume = maxVolume * windController.GetWindspeed();
    }
}
