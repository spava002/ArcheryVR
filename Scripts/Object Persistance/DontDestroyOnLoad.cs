using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

    void Awake() {
        // Ensure that only a single instance of this object is active at all times
        // Find any objects that have a dont destroy on load script
        DontDestroyOnLoad[] dontDestroyObjects = FindObjectsByType<DontDestroyOnLoad>(FindObjectsSortMode.None);

        if (dontDestroyObjects.Length > 0) {
            // Compare their instance id, and delete the ones with a higher instance id
            foreach (DontDestroyOnLoad dontDestroyObject in dontDestroyObjects) {
                if (this == dontDestroyObject) {
                    continue;
                }

                if (name == dontDestroyObject.name) {
                    if (GetInstanceID() > dontDestroyObject.GetInstanceID()) {
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }

        DontDestroyOnLoad(this);
    }
}
