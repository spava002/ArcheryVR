using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArrowPool : MonoBehaviour {
    [SerializeField] GameObject arrow;
    [SerializeField] int poolSize;
    [SerializeField] float despawnTime;
    [Tooltip("Despawn time used when multiple arrows are fired in a single turn, or arrows are fired out of turn. Faster time helps bring them back into the pool sooner.")]
    [SerializeField] float speedyDespawnTime;

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

    public List<ArrowController> GetActiveArrowControllers() {
        List<ArrowController> activeArrowControllers = new List<ArrowController>();
        foreach (GameObject arrow in arrows) {
            if (arrow.activeSelf) {
                activeArrowControllers.Add(arrow.GetComponent<ArrowController>());
            }
        }
        return activeArrowControllers;
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
    public void InitiateDespawnTimer(GameObject arrow, bool speedUpDespawn = false) {
        if (speedUpDespawn) {
            StartCoroutine(DespawnArrow(arrow, speedyDespawnTime));
        }
        else {
            StartCoroutine(DespawnArrow(arrow, despawnTime));
        }
        
    }

    IEnumerator DespawnArrow(GameObject arrow, float despawnTime) {
        float elapsedTime = 0f;
        ArrowController arrowController = arrow.GetComponent<ArrowController>();
        while (elapsedTime < despawnTime && !arrowController.GetIsGrabbed()) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (elapsedTime >= despawnTime) {
            arrow.GetComponentInChildren<MeshCollider>().enabled = true;
            arrowController.SetArrowOwner(PlayerType.None);
            arrow.transform.rotation = Quaternion.identity;
            arrow.SetActive(false);
        }
    }
}