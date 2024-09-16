using System.Collections;
using UnityEngine;

public class ArrowPool : MonoBehaviour {
    [SerializeField] GameObject arrow;
    [SerializeField] int poolSize;
    [SerializeField] float despawnTime;

    GameObject[] arrows;

    void Start() {
        arrows = new GameObject[poolSize];
        InstantiateArrows();
    }

    void InstantiateArrows() {
        for (int i = 0; i < poolSize; i++) {
            GameObject newArrow = Instantiate(arrow, transform);
            newArrow.SetActive(false);
            arrows[i] = newArrow;
        }
    }

    public GameObject GetAvailableArrow() {
        foreach (GameObject arrow in arrows) {
            if (!arrow.activeInHierarchy) {
                return arrow;
            }
        }
        return null;
    }

    // Starts the despawn timer
    public void InitiateDespawnTimer(GameObject arrow) {
        StartCoroutine(DespawnArrow(arrow));
    }

    IEnumerator DespawnArrow(GameObject arrow) {
        float elapsedTime = 0f;
        ArrowController arrowController = arrow.GetComponent<ArrowController>();
        while (elapsedTime < despawnTime && !arrowController.GetIsGrabbed()) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (elapsedTime >= despawnTime) {
            arrow.GetComponentInChildren<MeshCollider>().enabled = true;
            arrowController.SetArrowOwner(PlayerType.None);
            arrow.SetActive(false);
        }
    }
}