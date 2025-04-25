using UnityEngine;

public class SpawnPlayer : MonoBehaviour {
    void Start() {
        GameObject player = GameObject.FindWithTag("Player");

        if (player) {
            player.transform.position = transform.position;
        }
    }
}
