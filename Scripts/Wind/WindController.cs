using UnityEngine;

public class WindController : MonoBehaviour {
    [Header("Wind Settings")]
	[SerializeField] float maxWindspeed;
    [Tooltip("The amount of arrows that need to hit the target before a new wind is generated.")]
    [SerializeField] int windResetThreshold;

    TreeAnimationManager treeAnimationManager;
    MatchDisplay matchDisplay;
    CompassWindVisualizer compassWindVisualizer;
	ArrowController arrowController;
    Rigidbody arrowRb;
	float windSpeed;
	Vector3 windDirection;
    float windAngle;
    readonly string[] cardinalDirections = {"W", "WNW", "NW", "NNW", "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W"};
    const float cardinalDirectionRange = 22.5f;
    int arrowTargetHits = 0;

	void Awake() {
        treeAnimationManager = FindAnyObjectByType<TreeAnimationManager>();
        InitializeWindController();

        // USED FOR TESTING PURPOSES
        // windspeed = maxWindspeed;
        // windDirection = new Vector3(-1f, 0f, 1f);
        // windAngle = Vector3.SignedAngle(Vector3.right, windDirection, Vector3.up);
	}

    // Calling through fixed update rather than update makes it so updates are made on every physics call within the engine
    // This way the calls are frame-rate independent
	void FixedUpdate() {
        // Generates new winds if the threshold of targets have been hit
        if (arrowTargetHits > windResetThreshold) {
            GenerateWindspeed();
            GenerateWindDirection();

            if (!matchDisplay) {
                matchDisplay = FindAnyObjectByType<MatchDisplay>();
            }
            matchDisplay.UpdateAndDisplayWindInfo();

            if (!compassWindVisualizer) {
                compassWindVisualizer = FindAnyObjectByType<CompassWindVisualizer>();
            }
            StartCoroutine(compassWindVisualizer.UpdateNeedleRotation(windAngle));

            treeAnimationManager.UpdateTreeAnimations(windSpeed, maxWindspeed);

            arrowTargetHits = 0;
        }

        if (arrowController != null && arrowRb != null) {
            if (arrowController.GetIsMoving()) {
                arrowRb.AddForce(windDirection * windSpeed);
            }
            else {
                arrowController = null;
                arrowRb = null;
            }
        }
	}

    public void InitializeWindController() {
        arrowTargetHits = 0;
        GenerateWindspeed();
        GenerateWindDirection();

        treeAnimationManager.UpdateTreeAnimations(windSpeed, maxWindspeed);
    }

	void GenerateWindspeed() {
        float minRange = 0f;
        float maxRange = 1f;
        float maxDifficultyWind = 1f;

        WindDifficulty windDifficulty = (WindDifficulty)PlayerPrefs.GetInt("WindDifficulty");

        // Set the ranges and the max windspeed that the difficulty can reach
        switch (windDifficulty) {
            case WindDifficulty.PracticeWind:
                minRange = 0f;
                maxRange = 1f;
                maxDifficultyWind = maxWindspeed;
                break;
            case WindDifficulty.EasyWind:
                minRange = 0f;
                maxRange = 1f;
                maxDifficultyWind = 1f;
                break;
            case WindDifficulty.NormalWind:
                minRange = 0.25f;
                maxRange = 1f;
                maxDifficultyWind = 2f;
                break;
            case WindDifficulty.HardWind:
                minRange = 0.5f;
                maxRange = 1f;
                maxDifficultyWind = maxWindspeed;
                break;
        }

        // Randomly generate the windspeed using ranges and max wind for the difficulty
        windSpeed = Random.Range(minRange, maxRange) * maxDifficultyWind;
	}

	void GenerateWindDirection() {
        // Wind only goes in X and Z directions
        windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        // Calculates the angle the wind is pointing in, is made use of later when converting to cardinal direction
        windAngle = Vector3.SignedAngle(Vector3.right, windDirection, Vector3.up);
	}

    // Gets the necessary arrow components to apply wind to them via FixedUpdate()
    public void InitializeWindEffect(GameObject arrow) {
    	arrowController = arrow.GetComponent<ArrowController>();
        arrowRb = arrow.GetComponent<Rigidbody>();
    }

	// To display the wind direction to the player
	public string GetCardinalWindDirection() {
        // Since signed angles return between -180 to 180, we need to add 180 to normalize it to a standard 0 to 360 angle
        // Divide the normalized windAngle by the range assigned to each direction to get the direction that angle points in
        int index = Mathf.RoundToInt((windAngle + 180f) / cardinalDirectionRange);
        return cardinalDirections[index];
	}

    public float GetTruncatedWindspeed() {
        return (int)(windSpeed * 100f) / 100f;
    }

    public float GetWindAngle() {
        return windAngle;
    }

    public void IncreaseArrowTargetHits() {
        arrowTargetHits++;
    }

    public float GetMaxWindspeed() {
        return maxWindspeed;
    }
}
