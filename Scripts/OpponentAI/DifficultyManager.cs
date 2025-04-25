using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour {
    [SerializeField] Difficulty[] difficulties;

    [System.Serializable]
    public class Difficulty {
        public DifficultyLevel difficultyLevel;

        [Header("Outer Ranges")]
        [Tooltip("Max angle offset that arrow can be fired at for the outer range.")]
        public float maxOuterRange;
        [Tooltip("Min angle offset that arrow can be fired at for the outer range. (Reccomended value is Max Outer Range / 2)")]
        public float minOuterRange;

        [Header("Inner Ranges")]
        [Tooltip("Max angle offset that arrow can be fired at for the inner range. (Reccomended value is Max Outer Range / 2)")]
        public float maxInnerRange;
        [Tooltip("Min angle offset that arrow can be fired at for the inner range. (Should not be less than 0)")]
        public float minInnerRange;

        [Header("Ramp Up Values")]
        [Tooltip("Minimum points difference required between players to ramp up the difficulty.")]
        public int rampUpRange;

        [Tooltip("The step at which the offset is increased/decreased when difficulty is being ramped up.")]
        public float offsetStep;

        public DifficultyLevel GetDifficultyLevel() {
            return difficultyLevel;
        }

        public float GetMaxOuterRange() {
            return maxOuterRange;
        }

        public float GetMinOuterRange() {
            return minOuterRange;
        }

        public float GetMaxInnerRange() {
            return maxInnerRange;
        }

        public float GetMinInnerRange() {
            return minInnerRange;
        }

        public int GetRampUpRange() {
            return rampUpRange;
        }

        public float GetOffsetStep() {
            return offsetStep;
        }
    }

    public Difficulty GetDifficulty(DifficultyLevel difficultyLevel) {
        foreach (Difficulty difficulty in difficulties) {
            if (difficultyLevel == difficulty.GetDifficultyLevel()) {
                return difficulty;
            }
        }
        return null;
    }

}
