using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBehaviourScript : MonoBehaviour {

    // Color macros
    readonly Color SUSCEPTIBLE_C = new Color(255/255f, 220/255f, 0f);
    //Color readonly INFECTED_C    = new Color(220/255f, 50/255f, 50/255f);
    //readonly Color RESISTANT_C   = new Color(145/255f, 50/255f, 220/255f);

    // The strain number that this person is infected with or resistant against (0 if susceptible to anything)
    public int strainNumber = 0;

    // The array of strain colors
    Color[] strainColors;

    // The number of frames until this person's resistance expires
    int resistanceExpires = 0;

    // Sprite renderer component
    SpriteRenderer sprite;

    // Probability variables
    [Range(0f, 100f)] public float chanceOfRecovery = 5f;
    [Range(0f, 100f)] public float chanceOfDeath = 5f;
    [Range(0f, 100f)] public float chanceOfRandomInfection = 5f;
    [Range(0f, 100f)] public float chanceOfSpreadOnContact = 100f;

    // Energy level of the person
    public int energy = 0;

    // Amount of energy required to produce an offspring
    public int offspringEnergy = 100;

    // The number of frames that have passed
    int frame;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        // Get the sprite renderer
        sprite = transform.GetComponent<SpriteRenderer>();

        // Get the strain colors
        strainColors = GameObject.FindWithTag("GameController").GetComponent<GameControllerScript>().strainColors;

        // Initialize color
        if (transform.tag == "infected") {
            sprite.color = strainColors[strainNumber - 1];
        }
	}

    // Update is called once per frame
    void Update() {



        if (GameControllerScript.sceneActive)
        {
            // Ensure the person is the correct color
            if (transform.tag == "infected")
            {
                sprite.color = strainColors[strainNumber - 1];
            }
            else if (transform.tag == "resistant")
            {
                sprite.color = new Color(50 / 255f, 50 / 255f, 250 / 255f);
            }
            else
            {
                sprite.color = SUSCEPTIBLE_C;
            }

            // Check if the person has lost their resistance to the disease
            if (frame >= resistanceExpires * (GameControllerScript.updateFrequency / 200f) && resistanceExpires > 0)
            {
                resistanceExpires = 0;
                if (transform.tag == "resistant")
                {
                    transform.tag = "susceptible";
                    if (strainNumber > 0) { strainNumber--; }
                }
                frame = 0;
            }

            frame++;
        }
	}

    // +----------------+-----------------------------------------------------------------------------------------------------------------------------------------
    // | Status Changes |
    // +----------------+

    // Make this person produce one offspring
    void Reproduce() {
        energy -= offspringEnergy;

        GameObject newPerson = Instantiate(transform.gameObject);
        newPerson.GetComponent<SpriteRenderer>().color = SUSCEPTIBLE_C;
        newPerson.GetComponent<PersonBehaviourScript>().energy = 0;
        newPerson.transform.parent = transform.parent;
        newPerson.tag = "susceptible";
        newPerson.name = "Person";
    }

    // Make this person survive the infection
    void Recover() {
        transform.tag = "resistant";
        sprite.color = SUSCEPTIBLE_C;
        resistanceExpires = 2000;
        frame = 0;
    }

    // Kill this person
    void Die() {
        Destroy(transform.gameObject);
    }

    // Infect this person
    void Infect(int strain) {
        if (strain != strainNumber) {
            sprite.color = strainColors[strain - 1];

            strainNumber = strain;
            transform.tag = "infected";
        }
    }

    // +--------------+-------------------------------------------------------------------------------------------------------------------------------------------
    // | OnGameUpdate |
    // +--------------+

    void OnGameUpdate() {
        // Each person gets between 5 and 15 energy on every update if reproduction turned on
        if (GameControllerScript.canReproduce) {
            energy += Random.Range(5, 15);
        }

        switch (transform.tag) {
            case "infected":
                // Possibly die or recover if infected

                if (Random.value < chanceOfRecovery / 100f) {
                    Recover();
                } else if (Random.value < chanceOfDeath / 100f) {
                    Die();
                }
                break;

            case "susceptible":
                // Possibly become randomly infected with original strain if susceptible

                if (Random.value < chanceOfRandomInfection / 100f) { 
                    Infect(1);
                }
                break;

            case "resistant":
                // Possibly become randomly infected with next strain if resistant (smaller chance)

                if (strainNumber < GameControllerScript.maxStrain && Random.value < chanceOfRandomInfection / 200f) { 
                    Infect(strainNumber + 1);
                }
                break;
        }

        // Reproduce if allowed
        if (energy >= offspringEnergy && GameControllerScript.canReproduce) {
            Reproduce();
        }
    }

	// +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
	// | Other |
	// +-------+

	// Called when this person passes another person
	void OnTriggerEnter2D(Collider2D collision) {
        if (sprite) {
            if (collision.gameObject.tag == "infected" && Random.value < chanceOfSpreadOnContact / 100f) {    
                // Collided with an infected person

                int strain = collision.gameObject.GetComponent<PersonBehaviourScript>().strainNumber;
                if (strain > strainNumber) {
                    Infect(strain);
                }
            }
        }
	}

	// Called when this person is treated
	void OnTreat() {
        if (transform.tag == "infected") {
            // The person is infected, so treat them

            transform.tag = "resistant";
            sprite.color = SUSCEPTIBLE_C;

            // If treatments are not vaccines, then resistance expires after 500 frames
            if (!GameControllerScript.treatmentsAsVaccines) {
                resistanceExpires = 4000;
                frame = 0;
            }
        }
    }
}
