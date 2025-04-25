using UnityEngine;

public class BowstringAudioController : MonoBehaviour {
    [SerializeField] AudioClip bowstringPull;

    AudioSource audioSource;
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBowstringPull() {
        if (!audioSource.isPlaying) {
            // Randomize pitch so it doesnt sound the same every time
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.volume = 0.75f;
            audioSource.clip = bowstringPull;
            audioSource.Play();
        }
    }

    // Allows the audio to be stopped, since it will be running on a loop
    public void StopBowstringPull() {
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
    }
}
