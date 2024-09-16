using UnityEngine;

public class SimulationAI : MonoBehaviour {
    void Start() {
        FindAnyObjectByType<ArrowPathOptimizer>().CalculateOptimalArrowPath();
    }
}
