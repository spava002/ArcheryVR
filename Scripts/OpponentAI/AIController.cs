using UnityEngine;

public class AIController : MonoBehaviour {
    [SerializeField] GameObject bow;
    [SerializeField] GameObject bowstringMidpoint;

    [Header("Firing Speeds")]
    [SerializeField] float alignmentSpeed;
    [SerializeField] float drawBackSpeed;

    ArrowPool arrowPool;
    ArrowPathOptimizer arrowPathOptimizer;
    TurnManager turnManager;
    DifficultyManager difficultyManager;
    PointsManager pointsManager;
    DifficultyManager.Difficulty difficulty;
    PointsManager.Player[] players;
    Quaternion initialRotation;
    Vector3 arrowDrawbackPosition;
    BowstringInteractable bowstringInteractable;
    float bowstringDrawDistance;
    Vector3 bowstringDrawbackPosition;
    GameObject arrow;
    ArrowController arrowController;
    bool isAligned;
    bool hasBeenFired;
    bool atInitialRotation;
    bool offsetsCalculated;
    // Negative xOffset = Up & Positive xOffset = Down
    float xOffset;
    // Negative yOffset = Left & Positive yOffset = Right
    float yOffset;

    void Start() {
        arrowPool = FindAnyObjectByType<ArrowPool>();
        arrowPathOptimizer = FindAnyObjectByType<ArrowPathOptimizer>();
        turnManager = FindAnyObjectByType<TurnManager>();
        difficultyManager = FindAnyObjectByType<DifficultyManager>();
        pointsManager = FindAnyObjectByType<PointsManager>();

        // Typecast the integer stored in PlayerPrefs back into an enum
        // If no value stored in Difficulty playerpref, then difficulty is "Easy" by default
        DifficultyLevel difficultyLevel = (DifficultyLevel)PlayerPrefs.GetInt("Difficulty");
        // DifficultyLevel difficultyLevel = DifficultyLevel.Normal;
        // Get the difficulty object pertaining to the difficulty level enum
        difficulty = difficultyManager.GetDifficulty(difficultyLevel);
        // Get the active players
        players = pointsManager.GetPlayers();

        // Sets the starting rotation of the AI, so that we can reset it back each time an arrow has been fired
        initialRotation = transform.rotation;
        
        bowstringInteractable = bowstringMidpoint.GetComponent<BowstringInteractable>();
        bowstringDrawDistance = Mathf.Abs(bowstringInteractable.GetOuterXDrawLimit());

        // The position the bowstring should reach when drawn back before firing the arrow
        bowstringDrawbackPosition = bowstringMidpoint.transform.position - new Vector3(0f, 0f, bowstringDrawDistance);
    }

    void Update() {
        // Check if an arrow has been assigned yet
        if (arrow == null) {
            AssignAndCalculateArrowPath();
            offsetsCalculated = false;
        }

        if (!offsetsCalculated && turnManager.GetTurn() == PlayerType.Player2) {
            // Offsets based on difficulty so arrow is fired off-center slightly
            CalculateOffsets();
        }

        // If more players are added down the line, a better practice would be to compare the GetTurn() to the last index of the enum, since it should always be the AI
        if (!arrowPathOptimizer.GetSimulationRunning() && !isAligned && turnManager.GetTurn() == PlayerType.Player2) {
            AlignBowAndArrow();
        }

        if (isAligned && !hasBeenFired) {
            DrawBackAndFireArrow();
        }

        if (hasBeenFired && !atInitialRotation) {
            ResetRotation();
        }
        
        // Check if AI has been set back to the initial rotation, has been fired, and has stopped moving (hit an object)
        if (atInitialRotation && hasBeenFired != arrowController.GetIsMoving()) {
            arrow = null;
            arrowController = null;
        }
    }

    void AssignAndCalculateArrowPath() {
        arrow = arrowPool.GetAvailableArrow();
        arrowController = arrow.GetComponent<ArrowController>();
        arrowController.SetArrowOwner(PlayerType.Player2);
        // Need to modify this position to the max bowstring length in the z axis, to give it an offset
        arrow.transform.position = transform.position + new Vector3(0f, 0f, bowstringDrawDistance);
        // Sets the drawback position to the 0,0,0 in local position of the AI Opponent
        arrowDrawbackPosition = transform.position;
        // Making the bow the parent of the arrow, so that they can both be rotated/adjusted together during the alignment phase
        arrow.transform.parent = bow.transform;
        arrow.SetActive(true);
        arrowPathOptimizer.CalculateOptimalArrowPath();
        isAligned = false;
        hasBeenFired = false;
    }

    void CalculateOffsets() {
        // Opponent, in this case being the real human player(s)
        int totalOpponentPoints = 0;
        int totalPoints = 0;
        foreach (PointsManager.Player player in players) {
            // Player 2 signifies the AI player, which we save seperately
            if (player.GetPlayerType() == PlayerType.Player2) {
                totalPoints = player.GetPoints();
            }
            else {
                totalOpponentPoints += player.GetPoints();
            }
        }

        float outerRange = difficulty.GetMaxOuterRange();
        float innerRange = difficulty.GetMinInnerRange();
        int offsetMultiplier = Mathf.Abs(totalPoints - totalOpponentPoints) - (difficulty.GetRampUpRange() - 1);

        // If the AI is winning, increase its inner range offset so its less likely to hit for higher points
        if (totalPoints - totalOpponentPoints >= difficulty.GetRampUpRange()) {
            innerRange = difficulty.GetMinInnerRange() + (difficulty.GetOffsetStep() * offsetMultiplier);
            // Constrain the inner range to only reach a maximum of the pre-defined max inner range, regardless of difference in points
            if (innerRange > difficulty.GetMaxInnerRange()) {
                innerRange = difficulty.GetMaxInnerRange();
            }
        }
        // If the AI is losing, decrease its outer range offset so its more likely to hit for higher points
        else if (totalOpponentPoints - totalPoints >= difficulty.GetRampUpRange()) {
            outerRange = difficulty.GetMaxOuterRange() - (difficulty.GetOffsetStep() * offsetMultiplier);
            // Constrain the outer range to only reach a minimum of the pre-defined min outer range, regardless of difference in points
            if (outerRange < difficulty.GetMinOuterRange()) {
                outerRange = difficulty.GetMinOuterRange();
            }
        }

        // Debug.Log(totalPoints - totalOpponentPoints + " points difference.");

        // Debug.Log("Using outer range: " + outerRange);
        // Debug.Log("Using inner range: " + innerRange);

        // Randomly generate a number within the ranges as an offset to the x and y axis firing angles
        xOffset = Mathf.Sign(Random.Range(-1, 1)) * Random.Range(outerRange, innerRange);
        yOffset = Mathf.Sign(Random.Range(-1, 1)) * Random.Range(innerRange, outerRange);

        // Debug.Log("xOffset: " + xOffset);
        // Debug.Log("yOffset: " + yOffset);

        offsetsCalculated = true;
    }

    void AlignBowAndArrow() {
        // Randomized rotation offsets applied to optimal rotation here based on chosen difficulty
        Quaternion optimalRotation = arrowPathOptimizer.GetOptimalRotation();
        // optimalRotation = Quaternion.Euler(optimalRotation.eulerAngles.x, optimalRotation.eulerAngles.y, optimalRotation.eulerAngles.z);
        optimalRotation = Quaternion.Euler(optimalRotation.eulerAngles.x + xOffset, optimalRotation.eulerAngles.y + yOffset, optimalRotation.eulerAngles.z);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, optimalRotation, alignmentSpeed * Time.deltaTime);
        if (Quaternion.Angle(transform.rotation, optimalRotation) <= Mathf.Epsilon) {
            // Since RotateTowards overshoots a little, and we need a precise rotation, we just rotate to the optimal rotation since we are close enough
            transform.rotation = optimalRotation;
            isAligned = true;
            atInitialRotation = false;
        }
    }

    void DrawBackAndFireArrow() {
        bowstringInteractable.MidpointGrabbed();

        Vector3 arrowPosition = arrow.transform.position;
        Vector3 bowstringPosition = bowstringMidpoint.transform.position;

        // Move the arrow back to firing position
        arrow.transform.position = Vector3.MoveTowards(arrowPosition, arrowDrawbackPosition, drawBackSpeed * Time.deltaTime);
        // Move the bowstring midpoint back to firing position
        bowstringMidpoint.transform.position = Vector3.MoveTowards(bowstringPosition, bowstringDrawbackPosition, drawBackSpeed * Time.deltaTime);

        if (Vector3.Distance(arrowPosition, arrowDrawbackPosition) <= Mathf.Epsilon) {
            // Make the arrow's parent back to the arrow pool again so it is not affected by the rotation of the AI when it resets back to its initial position
            arrow.transform.parent = arrowPool.transform;
            arrowController.FireArrow(1f);
            // Releases the bowstring
            bowstringInteractable.MidpointReleased();
            hasBeenFired = true;
        }
    }

    void ResetRotation() {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, initialRotation, alignmentSpeed * Time.deltaTime);
        if (Quaternion.Angle(transform.rotation, initialRotation) <= Mathf.Epsilon) {
            // Since RotateTowards overshoots a little, and we need a precise rotation, we just rotate to the initialRotation since we are close enough
            transform.rotation = initialRotation;
            atInitialRotation = true;
        }
    }
}
