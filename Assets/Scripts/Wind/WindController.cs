using UnityEngine;

public class WindController : MonoBehaviour {
	[SerializeField] float maxWindspeed;
    [SerializeField] int windResetThreshold;

	ArrowController arrowController;
    Rigidbody arrowRb;
	float windspeed;
	Vector3 windDirection;
    float windAngle;
    string[] cardinalDirections = {"W", "WNW", "NW", "NNW", "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W"};
    float cardinalDirectionRange = 22.5f;
    int arrowTargetHits = 0;

	void Awake() {
        // This is setup for testing, remove and uncomment below lines when done
        windspeed = maxWindspeed;
        windDirection = new Vector3(-1f, 0f, 1f);
        windAngle = Vector3.SignedAngle(Vector3.right, windDirection, Vector3.up);
        // GenerateWindspeed();
        // GenerateWindDirection();
	}

    // Calling through fixed update rather than update makes it so updates are made on every physics call within the engine
    // This way the calls are frame-rate independent
	void FixedUpdate() {
        // Generates new winds if the threshold of targets have been hit
        if (arrowTargetHits >= windResetThreshold) {
            GenerateWindspeed();
            GenerateWindDirection();
            arrowTargetHits = 0;
        }

        if (arrowController != null && arrowRb != null) {
            if (arrowController.GetIsMoving()) {
                arrowRb.AddForce(windDirection * windspeed);
            }
            else {
                arrowController = null;
                arrowRb = null;
            }
        }
	}

	void GenerateWindspeed() {
        // Lower limit of range should increase/decrease based on difficulty selected
        windspeed = Random.Range(0.5f, 1f) * maxWindspeed;
	}

	void GenerateWindDirection() {
        // Wind only goes in X and Z directions
        windDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        // Calculates the angle the wind is pointing in, is made use of later when converting to cardinal direction
        windAngle = Vector3.SignedAngle(Vector3.right, windDirection, Vector3.up);
	}

    // Gets the necessary arrow components to apply wind to it
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

    public float GetWindspeed() {
        return windspeed;
    }

    public float GetWindAngle() {
        return windAngle;
    }

    public void IncreaseArrowTargetHits() {
        arrowTargetHits++;
    }
}
