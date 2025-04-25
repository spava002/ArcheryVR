using UnityEngine;

public class WindAudioPlayer : MonoBehaviour {
    [SerializeField] float minVolume;
    [SerializeField] float maxVolumeIncrease;

    WindController windController;
    AudioSource audioSource;

    void Start() {
        windController = FindAnyObjectByType<WindController>();
        audioSource = GetComponent<AudioSource>();

        audioSource.Play();
    }

    void Update() {
        audioSource.volume = minVolume + maxVolumeIncrease * (windController.GetTruncatedWindspeed() / windController.GetMaxWindspeed());
    }
}
