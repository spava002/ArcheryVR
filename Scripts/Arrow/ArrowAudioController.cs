using UnityEngine;

public class ArrowAudioController : MonoBehaviour {
    [Header("Audio Settings")]
    [SerializeField] float minPitch;
    [SerializeField] float maxPitch;
    [SerializeField] [Range(0f, 1f)] float volume;
    [SerializeField] AudioClip arrowFire;
    [SerializeField] AudioClip arrowHit;

    AudioSource audioSource;
    AudioEchoFilter audioEchoFilter;
    void Start() {
        audioSource = GetComponent<AudioSource>();
        audioEchoFilter = GetComponent<AudioEchoFilter>();
    }

    public void PlayArrowFire() {
        audioEchoFilter.enabled = false;
        // Randomize pitch so it doesnt sound the same every time
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        if (!audioSource.enabled) {
            Debug.Log("Audio was disabled");
            audioSource.enabled = true;
        }
        audioSource.PlayOneShot(arrowFire, volume);
        
    }

    // public void PlayArrowFire() {
    //     if (!audioSource.gameObject.activeInHierarchy) {
    //         Debug.Log("AudioSource GameObject is inactive in the hierarchy!");
    //         audioSource.gameObject.SetActive(true);
    //     }

    //     audioEchoFilter.enabled = false;
    //     // Randomize pitch so it doesn't sound the same every time
    //     audioSource.pitch = Random.Range(0.8f, 1.2f);

    //     if (!audioSource.enabled) {
    //         Debug.Log("AudioSource component was disabled. Re-enabling.");
    //         audioSource.enabled = true;
    //     }

    //     audioSource.PlayOneShot(arrowFire, 0.75f);
    // }


    public void PlayArrowHit() {
        audioEchoFilter.enabled = true;
        if (!audioSource.enabled) {
            Debug.Log("Audio was disabled");
            audioSource.enabled = true;
        }
        audioSource.PlayOneShot(arrowHit, volume);
    }
}
