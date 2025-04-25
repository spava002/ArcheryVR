using UnityEngine;

public class QuiverManager : MonoBehaviour {
    SpawnArrow spawnArrow;

    void Awake () {
        spawnArrow = GetComponent<SpawnArrow>();
        spawnArrow.enabled = false;
    }

    public void ToggleSpawnArrow() {
        spawnArrow.enabled = !spawnArrow.enabled;
    }
}
