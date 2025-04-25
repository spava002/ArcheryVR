using UnityEngine;

// IMPORTANT: Must place target spawn prefab between the Opponent and Player so that target spawns at an equal distance from the two
public class SpawnTarget : MonoBehaviour {

    public void InitializeTargetDistance() {
        int zDistanceIncrease = 0;

        // Adjust the target's offset from the player spawn based on the TargetDistance, which is assigned when a difficulty is chosen
        TargetDistance targetDistance = (TargetDistance)PlayerPrefs.GetInt("TargetDistance");

        switch (targetDistance) {
            case TargetDistance.PracticeRange:
                break;
            case TargetDistance.EasyRange:
                zDistanceIncrease = 15;
                break;
            case TargetDistance.NormalRange:
                zDistanceIncrease = 20;
                break;
            case TargetDistance.HardRange:
                zDistanceIncrease = 25;
                break;
        }

        // Only = 0 when target distance is on Practice Range, inthis case we leave the position as is
        if (zDistanceIncrease != 0) {
            Vector3 targetSpawnPosition = FindAnyObjectByType<SpawnTarget>().gameObject.transform.position;
            // Gets the parent of the points board (which has the target hit detector), which is the target object itself
            GameObject target = FindAnyObjectByType<TargetHitDetector>().transform.parent.gameObject;

            target.transform.position = new Vector3(targetSpawnPosition.x, targetSpawnPosition.y, targetSpawnPosition.z + zDistanceIncrease);
        }



        // float distanceFromSpawn = minDistanceFromSpawn;
        // // Distance adjustment of -1 is only assigned when in Practice difficulty, so we keep the target as it is
        // if (distanceAdjustment != -1) {
        //     target.transform.position = new Vector3(targetSpawnPosition.x, targetSpawnPosition.y, targetSpawnPosition.z + distanceFromSpawn + (distanceIncrement * distanceAdjustment));
        // }
        
    }
}
