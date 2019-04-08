using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorScript : MonoBehaviour {

    // State of this scene, person becomes inactive if false
    bool sceneActive = true;

    // The animation sprites
    public Sprite[] sprites;

    // The animation frame number this person is currently at
    int animFrame;

    // The game frame number
    int frame;

    // The number of frames between sprite updates
    public int framerate;

    // +------------------+---------------------------------------------------------------------------------------------------------------------------------------
    // | Start and Update |
    // +------------------+

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (sceneActive) {
            if (frame >= framerate) {
                RunAnimation();
                frame = 0;
            }

            frame++;
        }
	}

    // +-------+--------------------------------------------------------------------------------------------------------------------------------------------------
    // | Other |
    // +-------+

    // Move the animation to the next frame
    void RunAnimation() {
        animFrame = (animFrame + 1) % sprites.Length;
        if (transform.GetComponent<SpriteRenderer>()) {
            transform.GetComponent<SpriteRenderer>().sprite = sprites[animFrame];
        } else if (transform.GetComponent<Image>()) {
            transform.GetComponent<Image>().sprite = sprites[animFrame];
        }
    }

    // Flip the animation if need be
    void FlipAnimation(float xDirection) {
        if (transform.GetComponent<SpriteRenderer>()) {
            if (xDirection < float.Epsilon && xDirection > -float.Epsilon) {
                (transform.GetComponent<SpriteRenderer>()).flipX = (Random.value > 0.5f);
            } else {
                (transform.GetComponent<SpriteRenderer>()).flipX = (xDirection > 0);
            }
        }
    }

    // Sets the activeness of the scene
    void SetSceneActive(bool active) {
        sceneActive = active;
    }
}
