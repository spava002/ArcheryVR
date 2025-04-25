using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerBoundaryController : MonoBehaviour {
    [Header("Movement Information")]
    [SerializeField] InputAction moveAction;
    [SerializeField] float moveSpeed;

    [Tooltip("The maximum distance the player can move away from the spawn.")]
    [SerializeField] float maxMoveDistance;

    GameObject player;
    SpawnPlayer playerSpawn;
    ContinuousMoveProviderBase moveProvider;

    void Start() {
        player = gameObject;
        playerSpawn = FindAnyObjectByType<SpawnPlayer>();
        moveProvider = FindAnyObjectByType<ContinuousMoveProviderBase>();
    }

    void OnEnable() {
        moveAction.Enable();   
    }

    void OnDisable() {
        moveAction.Disable();    
    }

    void Update() {
        // Since the player persists through scenes, when a new scene is loaded, it needs to find the reference to that scene's player spawn
        if (!playerSpawn) {
            playerSpawn = FindAnyObjectByType<SpawnPlayer>();
        }
        float xMoveValue = moveAction.ReadValue<Vector2>().x;
        float yMoveValue = moveAction.ReadValue<Vector2>().y;
        Vector3 playerPosition = player.transform.position;
        Vector3 futurePlayerPosition = new Vector3(playerPosition.x + (xMoveValue * moveSpeed), playerPosition.y, playerPosition.z + (yMoveValue * moveSpeed));

        float futurePlayerDistanceToSpawn = Vector3.Distance(futurePlayerPosition, playerSpawn.transform.position);
        float playerDistanceToSpawn = Vector3.Distance(player.transform.position, playerSpawn.transform.position);
        // Debug.Log("Future: " + futurePlayerDistanceToSpawn);
        // Debug.Log("Current:" + playerDistanceToSpawn);

        // Calculates the future position of the player based on the move action trigger
        // If the future position is within the max distance, then we enable the player's movespeed so they can move around
        if (futurePlayerDistanceToSpawn < maxMoveDistance) {
            moveProvider.moveSpeed = moveSpeed;
        }
        // If the player goes beyond the max move distance, then we disable their move speed, so they cant keep moving in that direction
        else if (playerDistanceToSpawn > maxMoveDistance) {
            moveProvider.moveSpeed = 0f;
        }
    }

    void OnDrawGizmos() {
        if (playerSpawn != null) {
            Gizmos.DrawWireSphere(playerSpawn.transform.position, maxMoveDistance);
        }
    }
}
